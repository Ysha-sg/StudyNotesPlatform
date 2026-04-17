using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Data;
using StudyNotesPlatform.Models;
using System.Security.Claims;

namespace StudyNotesPlatform.Controllers;

// DTO для загрузки конспекта
public class UploadNoteModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public IFormFile File { get; set; } = null!;
}

// DTO для модерации
public class ModerateModel
{
    public int StatusId { get; set; }
    public string? Comment { get; set; }
}

// DTO для обновления конспекта
public class UpdateNoteModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public NotesController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: api/notes/my - мои конспекты
    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notes = await _context.Notes
            .Where(n => n.UserId == userId)
            .Include(n => n.Status)
            .Include(n => n.Teacher)
            .Include(n => n.University)
            .Include(n => n.Subject)
            .OrderByDescending(n => n.UploadedAt)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.Description,
                n.UploadedAt,
                Status = new { n.Status!.Code, n.Status.Name },
                Teacher = n.Teacher != null ? n.Teacher.FullName : "",
                University = n.University != null ? n.University.Name : "",
                Subject = n.Subject != null ? n.Subject.Name : "",
                n.DownloadsCount,
                n.AverageRating
            })
            .ToListAsync();
        return Ok(notes);
    }

    // GET: api/notes/download-history - история скачиваний
    [HttpGet("download-history")]
    [Authorize]
    public async Task<IActionResult> GetDownloadHistory()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Получаем конспекты, которые скачал пользователь
        // (предполагаем, что есть записи в таблице downloads)
        // Пока возвращаем конспекты пользователя с сортировкой по дате
        var history = await _context.Notes
            .Where(n => n.UserId == userId)
            .Include(n => n.University)
            .Include(n => n.Subject)
            .Include(n => n.Teacher)
            .OrderByDescending(n => n.UploadedAt)
            .Select(n => new
            {
                n.Id,
                n.Title,
                Subject = n.Subject != null ? n.Subject.Name : "",
                Teacher = n.Teacher != null ? n.Teacher.FullName : "",
                University = n.University != null ? n.University.Name : "",
                Rating = n.AverageRating ?? 0,
                DownloadsCount = n.DownloadsCount,
                DownloadedAt = n.UploadedAt
            })
            .ToListAsync();

        return Ok(history);
    }

    // POST: api/notes/upload - загрузка конспекта
    [HttpPost("upload")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> UploadNote([FromForm] UploadNoteModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Unauthorized();

        var subject = await _context.Subjects.FindAsync(model.SubjectId);
        if (subject == null)
            return BadRequest("Предмет не найден");
        if (subject.UniversityId != user.UniversityId)
            return BadRequest("Предмет не принадлежит вашему университету");

        if (model.TeacherId.HasValue)
        {
            var teacher = await _context.Teachers.FindAsync(model.TeacherId.Value);
            if (teacher == null)
                return BadRequest("Преподаватель не найден");
            if (teacher.UniversityId != user.UniversityId)
                return BadRequest("Преподаватель не принадлежит вашему университету");
        }

        if (model.File == null || model.File.Length == 0)
            return BadRequest("Файл не загружен");

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.File.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/{uniqueFileName}";

        var note = new Note
        {
            UserId = userId,
            UniversityId = user.UniversityId,
            SubjectId = model.SubjectId,
            TeacherId = model.TeacherId,
            StatusId = 1,
            Title = model.Title,
            Description = model.Description,
            FilePath = relativePath,
            UploadedAt = DateTime.UtcNow,
            DownloadsCount = 0,
            AverageRating = null
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            note.Id,
            note.Title,
            note.UploadedAt,
            Message = "Конспект успешно загружен и отправлен на модерацию"
        });
    }

    // POST: api/notes/moderate/{noteId} - модерация конспекта (только админ)
    [HttpPost("moderate/{noteId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ModerateNote(int noteId, [FromBody] ModerateModel model)
    {
        var note = await _context.Notes.FindAsync(noteId);
        if (note == null) return NotFound();

        var newStatus = await _context.NoteStatuses.FindAsync(model.StatusId);
        if (newStatus == null) return BadRequest("Статус не существует");

        note.StatusId = model.StatusId;
        await _context.SaveChangesAsync();

        var moderatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var log = new ModerationLog
        {
            NoteId = noteId,
            ModeratorId = moderatorId,
            Action = newStatus.Code,
            Comment = model.Comment,
            CreatedAt = DateTime.UtcNow
        };
        _context.ModerationLogs.Add(log);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Статус конспекта изменён на {newStatus.Name}" });
    }

    // GET: api/notes/all - все конспекты (только админ)
    [HttpGet("all")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllNotes([FromQuery] int? statusId = null)
    {
        var query = _context.Notes
            .Include(n => n.User)
            .Include(n => n.Status)
            .AsQueryable();
        if (statusId.HasValue)
            query = query.Where(n => n.StatusId == statusId.Value);
        var notes = await query
            .OrderByDescending(n => n.UploadedAt)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.UploadedAt,
                UserEmail = n.User!.Email,
                Status = n.Status!.Name,
                n.DownloadsCount,
                n.AverageRating
            })
            .ToListAsync();
        return Ok(notes);
    }

    // GET: api/notes/statuses - список статусов
    [HttpGet("statuses")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatuses()
    {
        var statuses = await _context.NoteStatuses
            .Select(s => new { s.Id, s.Code, s.Name, s.Description })
            .ToListAsync();
        return Ok(statuses);
    }

    // GET: api/notes - главный каталог (только одобренные конспекты)
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetNotes(
        [FromQuery] string? search,
        [FromQuery] string? university,
        [FromQuery] string? subject,
        [FromQuery] string? teacher)
    {
        var query = _context.Notes
            .Include(n => n.University)
            .Include(n => n.Subject)
            .Include(n => n.Teacher)
            .Where(n => n.StatusId == 2);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(n => n.Title.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(university))
        {
            query = query.Where(n => n.University != null && n.University.Name == university);
        }

        if (!string.IsNullOrWhiteSpace(subject))
        {
            query = query.Where(n => n.Subject != null && n.Subject.Name == subject);
        }

        if (!string.IsNullOrWhiteSpace(teacher))
        {
            query = query.Where(n => n.Teacher != null && n.Teacher.FullName.Contains(teacher));
        }

        var notes = await query
            .OrderByDescending(n => n.AverageRating ?? 0)
            .Select(n => new
            {
                n.Id,
                n.Title,
                Subject = n.Subject != null ? n.Subject.Name : "",
                Teacher = n.Teacher != null ? n.Teacher.FullName : "",
                University = n.University != null ? n.University.Name : "",
                Rating = n.AverageRating ?? 0,
                DownloadsCount = n.DownloadsCount
            })
            .ToListAsync();

        return Ok(notes);
    }

    // GET: api/notes/{id} - получить один конспект
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNote(int id)
    {
        var note = await _context.Notes
            .Include(n => n.University)
            .Include(n => n.Subject)
            .Include(n => n.Teacher)
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (note == null)
            return NotFound(new { message = "Конспект не найден" });

        return Ok(new
        {
            note.Id,
            note.Title,
            note.Description,
            Subject = note.Subject?.Name ?? "",
            Teacher = note.Teacher?.FullName ?? "",
            University = note.University?.Name ?? "",
            note.FilePath,
            note.DownloadsCount,
            Rating = note.AverageRating ?? 0,
            Author = note.User?.FullName,
            UploadedAt = note.UploadedAt
        });
    }

    // PUT: api/notes/{id} - обновление конспекта
    [HttpPut("{id}")]
    [Authorize(Roles = "student,admin")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteModel model)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null)
            return NotFound(new { message = "Конспект не найден" });

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (note.UserId != userId && userRole != "admin")
            return Forbid("Вы можете редактировать только свои конспекты");

        if (!string.IsNullOrWhiteSpace(model.Title))
            note.Title = model.Title;

        if (model.Description != null)
            note.Description = model.Description;

        if (model.SubjectId.HasValue)
        {
            var subject = await _context.Subjects.FindAsync(model.SubjectId.Value);
            if (subject == null)
                return BadRequest(new { message = "Предмет не найден" });
            note.SubjectId = model.SubjectId.Value;
        }

        if (model.TeacherId.HasValue)
        {
            var teacher = await _context.Teachers.FindAsync(model.TeacherId.Value);
            if (teacher == null)
                return BadRequest(new { message = "Преподаватель не найден" });
            note.TeacherId = model.TeacherId.Value;
        }

        if (note.StatusId == 2)
        {
            note.StatusId = 1;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Конспект обновлён. Если он был одобрен, отправлен на повторную модерацию",
            note.Id,
            note.Title,
            note.StatusId
        });
    }

    // DELETE: api/notes/{id} - удаление конспекта
    [HttpDelete("{id}")]
    [Authorize(Roles = "student,admin")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null)
            return NotFound(new { message = "Конспект не найден" });

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (note.UserId != userId && userRole != "admin")
            return Forbid("Вы можете удалять только свои конспекты");

        if (!string.IsNullOrEmpty(note.FilePath))
        {
            var filePath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath,
                note.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Конспект успешно удалён" });
    }

    // POST: api/notes/{id}/favorite - добавить/удалить из избранного
    [HttpPost("{id}/favorite")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var note = await _context.Notes.FindAsync(id);
        if (note == null)
            return NotFound(new { message = "Конспект не найден" });

        var existingFavorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.NoteId == id);

        if (existingFavorite != null)
        {
            _context.Favorites.Remove(existingFavorite);
            await _context.SaveChangesAsync();
            return Ok(new { isFavorite = false, message = "Удалено из избранного" });
        }
        else
        {
            var favorite = new Favorite
            {
                UserId = userId,
                NoteId = id,
                CreatedAt = DateTime.UtcNow
            };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return Ok(new { isFavorite = true, message = "Добавлено в избранное" });
        }
    }

    // GET: api/notes/favorites - получить избранные конспекты
    [HttpGet("favorites")]
    [Authorize]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Note)
                .ThenInclude(n => n.University)
            .Include(f => f.Note)
                .ThenInclude(n => n.Subject)
            .Include(f => f.Note)
                .ThenInclude(n => n.Teacher)
            .Include(f => f.Note)
                .ThenInclude(n => n.Status)
            .Select(f => new
            {
                f.Note.Id,
                f.Note.Title,
                Subject = f.Note.Subject != null ? f.Note.Subject.Name : "",
                Teacher = f.Note.Teacher != null ? f.Note.Teacher.FullName : "",
                University = f.Note.University != null ? f.Note.University.Name : "",
                Rating = f.Note.AverageRating ?? 0,
                DownloadsCount = f.Note.DownloadsCount,
                Status = f.Note.Status != null ? f.Note.Status.Name : ""
            })
            .ToListAsync();

        return Ok(favorites);
    }
}