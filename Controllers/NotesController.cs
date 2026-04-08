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

    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notes = await _context.Notes
            .Where(n => n.UserId == userId)
            .Include(n => n.Status)
            .OrderByDescending(n => n.UploadedAt)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.Description,
                n.UploadedAt,
                Status = new { n.Status!.Code, n.Status.Name },
                n.DownloadsCount,
                n.AverageRating
            })
            .ToListAsync();
        return Ok(notes);
    }

    [HttpPost("upload")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> UploadNote([FromForm] UploadNoteModel model)
    {
        // 1. Получаем текущего пользователя
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Unauthorized();

        // 2. Валидация предмета
        var subject = await _context.Subjects.FindAsync(model.SubjectId);
        if (subject == null)
            return BadRequest("Предмет не найден");
        if (subject.UniversityId != user.UniversityId)
            return BadRequest("Предмет не принадлежит вашему университету");

        // 3. Валидация преподавателя (если указан)
        if (model.TeacherId.HasValue)
        {
            var teacher = await _context.Teachers.FindAsync(model.TeacherId.Value);
            if (teacher == null)
                return BadRequest("Преподаватель не найден");
            if (teacher.UniversityId != user.UniversityId)
                return BadRequest("Преподаватель не принадлежит вашему университету");
        }

        // 4. Сохраняем файл на диск
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

        // 5. Создаём запись в БД
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

    [HttpGet("statuses")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatuses()
    {
        var statuses = await _context.NoteStatuses
            .Select(s => new { s.Id, s.Code, s.Name, s.Description })
            .ToListAsync();
        return Ok(statuses);
    }
}