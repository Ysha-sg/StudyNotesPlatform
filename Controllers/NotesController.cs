using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Data;
using StudyNotesPlatform.Models;
using UglyToad.PdfPig;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;

namespace StudyNotesPlatform.Controllers;

public class UploadNoteModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public IFormFile? File { get; set; }
}

public class ModerateModel
{
    public int StatusId { get; set; }
    public string? Comment { get; set; }
}

public class UpdateNoteModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
}

public class CreateComplaintModel
{
    public string Reason { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

public class ResolveComplaintModel
{
    public bool ConfirmComplaint { get; set; }
    public string? Comment { get; set; }
}

public class RateNoteModel
{
    public int Rating { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    private const int AutoCheckWordLimit = 2000;
    private const int MaxTextCharsForCheck = 24_000;
    private const int MaxBytesForHeuristicExtraction = 2 * 1024 * 1024;
    private const long MaxAutoTextExtractionFileSizeBytes = 20L * 1024 * 1024;

    private static readonly HashSet<string> AllowedUploadExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf"
    };

    private static readonly string[] ForbiddenKeywordStems =
    {
        "ответ",
        "тест"
    };

    private static readonly HashSet<string> TextLikeExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt",
        ".md",
        ".csv",
        ".json",
        ".xml",
        ".html",
        ".htm",
        ".rtf",
        ".log"
    };

    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "и", "или", "а", "но", "в", "во", "на", "по", "к", "ко", "из", "за", "от", "до", "для", "с", "со",
        "о", "об", "про", "у", "не", "это", "как", "что", "чтобы", "при", "под", "над", "без", "же", "ли",
        "the", "and", "or", "for", "with", "from", "into", "about", "this", "that", "are", "was", "were"
    };

    public NotesController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    private string GetPrivateUploadsDirectory()
    {
        return Path.Combine(_env.ContentRootPath, "uploads");
    }

    private static string ExtractStoredFileName(string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
        {
            return string.Empty;
        }

        return Path.GetFileName(storedPath.Replace('\\', '/'));
    }

    private string? ResolveNoteFilePath(Note note)
    {
        var fileName = ExtractStoredFileName(note.FilePath);
        var candidates = new List<string>();

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            candidates.Add(Path.Combine(GetPrivateUploadsDirectory(), fileName));

            if (!string.IsNullOrWhiteSpace(_env.WebRootPath))
            {
                candidates.Add(Path.Combine(_env.WebRootPath, "uploads", fileName));
            }
        }

        if (!string.IsNullOrWhiteSpace(note.FilePath))
        {
            var relative = note.FilePath.TrimStart('/', '\\')
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
            candidates.Add(Path.Combine(_env.ContentRootPath, relative));

            if (!string.IsNullOrWhiteSpace(_env.WebRootPath))
            {
                candidates.Add(Path.Combine(_env.WebRootPath, relative));
            }
        }

        foreach (var candidate in candidates.Distinct())
        {
            if (System.IO.File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private async Task<bool> CanUserAccessNoteFileAsync(Note note)
    {
        if (User.IsInRole("admin") || User.IsInRole("moderator"))
        {
            return true;
        }

        if (await IsStatusCodeAsync(note.StatusId, "approved"))
        {
            return true;
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId != null && int.TryParse(userId, out var parsedUserId) && note.UserId == parsedUserId;
    }

    private async Task<bool> IsStatusCodeAsync(int statusId, string code)
    {
        return await _context.NoteStatuses.AnyAsync(s => s.Id == statusId && s.Code == code);
    }

    private async Task<int?> GetStatusIdAsync(string code)
    {
        return await _context.NoteStatuses
            .Where(s => s.Code == code)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync();
    }

    private async Task<int?> GetSystemModeratorIdAsync()
    {
        return await (from user in _context.Users
                      join role in _context.Roles on user.RoleId equals role.Id
                      where role.Code == "moderator" || role.Code == "admin"
                      orderby user.Id
                      select (int?)user.Id).FirstOrDefaultAsync();
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        return Regex.Matches(text, @"\p{L}+", RegexOptions.CultureInvariant).Count;
    }

    private static IEnumerable<string> TokenizeWords(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        foreach (Match match in Regex.Matches(text, @"\p{L}+", RegexOptions.CultureInvariant))
        {
            var token = match.Value.ToLowerInvariant();
            if (token.Length >= 4 && !StopWords.Contains(token))
            {
                yield return token;
            }
        }
    }

    private static bool ContainsForbiddenKeyword(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        foreach (var token in TokenizeWords(text))
        {
            foreach (var stem in ForbiddenKeywordStems)
            {
                if (token.StartsWith(stem, StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasMeaningfulText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return Regex.IsMatch(text, @"\p{L}{3,}", RegexOptions.CultureInvariant);
    }

    private static string DecodePdfHexPayload(string hexPayload)
    {
        if (string.IsNullOrWhiteSpace(hexPayload))
        {
            return string.Empty;
        }

        var sanitized = Regex.Replace(hexPayload, @"\s+", string.Empty);
        if (sanitized.Length < 4)
        {
            return string.Empty;
        }

        if (sanitized.Length % 2 != 0)
        {
            sanitized = sanitized[..^1];
        }

        var bytes = new byte[sanitized.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            if (!byte.TryParse(sanitized.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber, null, out var parsed))
            {
                return string.Empty;
            }

            bytes[i] = parsed;
        }

        var candidates = new List<string>(4);
        try { candidates.Add(Encoding.BigEndianUnicode.GetString(bytes)); } catch { }
        try { candidates.Add(Encoding.Unicode.GetString(bytes)); } catch { }
        try { candidates.Add(Encoding.UTF8.GetString(bytes)); } catch { }
        try { candidates.Add(Encoding.ASCII.GetString(bytes)); } catch { }

        var best = candidates
            .Where(HasMeaningfulText)
            .OrderByDescending(c => Regex.Matches(c, @"\p{L}+", RegexOptions.CultureInvariant).Count)
            .FirstOrDefault();

        return best ?? string.Empty;
    }

    private static async Task<bool> HasPdfSignatureAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length < 5)
        {
            return false;
        }

        await using var stream = file.OpenReadStream();
        var signature = new byte[5];
        var read = await stream.ReadAsync(signature.AsMemory(0, signature.Length), cancellationToken);
        if (read < signature.Length)
        {
            return false;
        }

        return Encoding.ASCII.GetString(signature) == "%PDF-";
    }

    private static async Task<string> ExtractTextContentAsync(string absolutePath, string extension, CancellationToken cancellationToken)
    {
        if (TextLikeExtensions.Contains(extension))
        {
            await using var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

            var buffer = new char[MaxTextCharsForCheck];
            var readCount = await reader.ReadBlockAsync(buffer, 0, buffer.Length);
            return new string(buffer, 0, readCount);
        }

        if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                using var document = PdfDocument.Open(absolutePath);
                var parsedText = new StringBuilder();

                foreach (var page in document.GetPages())
                {
                    var pageText = page.Text;
                    if (!string.IsNullOrWhiteSpace(pageText))
                    {
                        parsedText.Append(' ').Append(pageText);
                    }

                    if (parsedText.Length >= MaxTextCharsForCheck)
                    {
                        break;
                    }
                }

                var normalized = parsedText.ToString();
                if (HasMeaningfulText(normalized))
                {
                    return normalized.Length > MaxTextCharsForCheck
                        ? normalized[..MaxTextCharsForCheck]
                        : normalized;
                }
            }
            catch
            {
                // Fallback below
            }

            var info = new FileInfo(absolutePath);
            var byteCount = (int)Math.Min(info.Length, MaxBytesForHeuristicExtraction);
            var bytes = new byte[byteCount];

            await using var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var read = await stream.ReadAsync(bytes.AsMemory(0, byteCount), cancellationToken);
            var raw = Encoding.ASCII.GetString(bytes, 0, read);

            var sb = new StringBuilder();
            foreach (Match match in Regex.Matches(raw, @"\(([^)]{2,})\)"))
            {
                var value = match.Groups[1].Value;
                if (value.Length > 0)
                {
                    sb.Append(' ').Append(value);
                }

                if (sb.Length >= MaxTextCharsForCheck)
                {
                    break;
                }
            }

            if (sb.Length < MaxTextCharsForCheck)
            {
                foreach (Match match in Regex.Matches(raw, @"<([0-9A-Fa-f\s]{8,})>"))
                {
                    var decoded = DecodePdfHexPayload(match.Groups[1].Value);
                    if (!HasMeaningfulText(decoded))
                    {
                        continue;
                    }

                    sb.Append(' ').Append(decoded);
                    if (sb.Length >= MaxTextCharsForCheck)
                    {
                        break;
                    }
                }
            }

            if (sb.Length == 0)
            {
                foreach (Match match in Regex.Matches(raw, @"\p{L}{2,}", RegexOptions.CultureInvariant))
                {
                    sb.Append(' ').Append(match.Value);
                    if (sb.Length >= MaxTextCharsForCheck)
                    {
                        break;
                    }
                }
            }

            return sb.ToString();
        }

        return string.Empty;
    }

    private enum AutoCheckDecision
    {
        Approve,
        Reject,
        SendToModeration
    }

    private static AutoCheckDecision EvaluateDescriptionMatch(string? description, string extractedText)
    {
        var descriptionTokens = TokenizeWords(description).Distinct().ToHashSet();
        var contentTokens = TokenizeWords(extractedText).Distinct().ToHashSet();

        if (descriptionTokens.Count < 3 || contentTokens.Count < 10)
        {
            return AutoCheckDecision.SendToModeration;
        }

        var overlapCount = descriptionTokens.Intersect(contentTokens).Count();
        var overlapRatio = overlapCount / (double)descriptionTokens.Count;

        if (overlapCount >= 3 && overlapRatio >= 0.35)
        {
            return AutoCheckDecision.Approve;
        }

        if (overlapCount <= 1 || overlapRatio < 0.15)
        {
            return AutoCheckDecision.Reject;
        }

        return AutoCheckDecision.SendToModeration;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rejectionActions = new[] { "rejected", "auto_rejected" };
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
                n.AverageRating,
                RejectionReason = _context.ModerationLogs
                    .Where(l => l.NoteId == n.Id && rejectionActions.Contains(l.Action) && l.Comment != null && l.Comment != "")
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(l => l.Comment)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(notes);
    }

    [HttpGet("download-history")]
    [Authorize]
    public async Task<IActionResult> GetDownloadHistory()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var history = await _context.NoteDownloads
            .Where(d => d.UserId == userId)
            .Include(d => d.Note)
                .ThenInclude(n => n!.University)
            .Include(d => d.Note)
                .ThenInclude(n => n!.Subject)
            .Include(d => d.Note)
                .ThenInclude(n => n!.Teacher)
            .OrderByDescending(d => d.DownloadedAt)
            .Select(n => new
            {
                Id = n.NoteId,
                Title = n.Note != null ? n.Note.Title : "",
                Subject = n.Note != null && n.Note.Subject != null ? n.Note.Subject.Name : "",
                Teacher = n.Note != null && n.Note.Teacher != null ? n.Note.Teacher.FullName : "",
                University = n.Note != null && n.Note.University != null ? n.Note.University.Name : "",
                Rating = n.Note != null ? n.Note.AverageRating ?? 0 : 0,
                DownloadsCount = n.Note != null ? n.Note.DownloadsCount : 0,
                DownloadedAt = n.DownloadedAt
            })
            .ToListAsync();

        return Ok(history);
    }

    [HttpPost("upload")]
    [Authorize(Roles = "student")]
    [RequestSizeLimit(104857600)]
    [RequestFormLimits(MultipartBodyLengthLimit = 104857600, ValueLengthLimit = int.MaxValue)]
    public async Task<IActionResult> UploadNote([FromForm] UploadNoteModel model, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Требуется авторизация", detail: "Пользователь из токена не найден.");
        }

        var subject = await ResolveSubjectAsync(user, model.SubjectId, model.SubjectName, cancellationToken);

        if (subject == null)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Предмет не найден", detail: "Выберите предмет из списка вашего вуза.");
        }

        if (subject.UniversityId != user.UniversityId)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Предмет не принадлежит вашему университету");
        }

        var teacher = await ResolveTeacherAsync(user, model.TeacherId, model.TeacherName, cancellationToken);
        if (model.TeacherId.HasValue && teacher == null)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Преподаватель не найден", detail: "Выберите преподавателя из списка вашего вуза.");
        }

        if (teacher != null && teacher.UniversityId != user.UniversityId)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Преподаватель не принадлежит вашему университету");
        }

        if (model.File == null || model.File.Length == 0)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Файл не загружен");
        }

        var pendingStatusId = await GetStatusIdAsync("pending");
        var approvedStatusId = await GetStatusIdAsync("approved");
        var rejectedStatusId = await GetStatusIdAsync("rejected");

        if (!pendingStatusId.HasValue || !approvedStatusId.HasValue || !rejectedStatusId.HasValue)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Не настроены системные статусы модерации");
        }

        var extension = Path.GetExtension(model.File.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedUploadExtensions.Contains(extension))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Формат файла запрещён", detail: "Поддерживается только формат .pdf.");
        }

        if (!await HasPdfSignatureAsync(model.File, cancellationToken))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Формат файла запрещён", detail: "Файл должен быть корректным PDF-документом.");
        }

        var uploadsFolder = GetPrivateUploadsDirectory();
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.File.FileName)}";
        var absoluteFilePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
        {
            await model.File.CopyToAsync(stream, cancellationToken);
        }

        var containsSuspiciousKeywords =
            ContainsForbiddenKeyword(model.Title) ||
            ContainsForbiddenKeyword(model.Description) ||
            ContainsForbiddenKeyword(model.File.FileName);

        var skippedAutoCheckForLargeFile = model.File.Length > MaxAutoTextExtractionFileSizeBytes;
        var extractedText = string.Empty;

        if (!skippedAutoCheckForLargeFile)
        {
            try
            {
                extractedText = await ExtractTextContentAsync(absoluteFilePath, extension, cancellationToken);
            }
            catch
            {
                extractedText = string.Empty;
            }

            containsSuspiciousKeywords = containsSuspiciousKeywords || ContainsForbiddenKeyword(extractedText);
        }

        var relativePath = $"/uploads/{uniqueFileName}";
        var wordsCount = CountWords(extractedText);
        var autoDecision = AutoCheckDecision.SendToModeration;
        var systemComment = "Файл отправлен модератору на ручную проверку.";

        if (skippedAutoCheckForLargeFile)
        {
            systemComment = $"Файл больше {MaxAutoTextExtractionFileSizeBytes / (1024 * 1024)} МБ и отправлен модератору без автопроверки текста.";
        }
        else if (containsSuspiciousKeywords)
        {
            systemComment = "Найдены потенциально спорные ключевые слова. Конспект отправлен модератору на ручную проверку.";
        }
        else if (wordsCount > 0 && wordsCount <= AutoCheckWordLimit)
        {
            autoDecision = EvaluateDescriptionMatch(model.Description, extractedText);
            if (autoDecision == AutoCheckDecision.Approve)
            {
                systemComment = "Автопроверка: описание соответствует содержанию.";
            }
            else if (autoDecision == AutoCheckDecision.Reject)
            {
                systemComment = "Автопроверка: описание не соответствует содержанию.";
            }
            else
            {
                systemComment = "Автопроверка: недостаточно данных для решения, отправлено модератору.";
            }
        }
        else if (wordsCount > AutoCheckWordLimit)
        {
            systemComment = $"Файл содержит больше {AutoCheckWordLimit} слов и отправлен модератору.";
        }

        var statusId = pendingStatusId.Value;
        var actionCode = "sent_to_manual_moderation";
        var responseMessage = "Конспект успешно загружен и отправлен на модерацию";

        if (autoDecision == AutoCheckDecision.Approve)
        {
            statusId = approvedStatusId.Value;
            actionCode = "auto_approved";
            responseMessage = "Конспект успешно загружен и автоматически одобрен";
        }
        else if (autoDecision == AutoCheckDecision.Reject)
        {
            statusId = rejectedStatusId.Value;
            actionCode = "auto_rejected";
            responseMessage = "Конспект загружен, но автоматически отклонён из-за несоответствия описанию";
        }

        var note = new Note
        {
            UserId = userId,
            UniversityId = user.UniversityId,
            SubjectId = subject.Id,
            TeacherId = teacher?.Id,
            StatusId = statusId,
            Title = model.Title,
            Description = model.Description,
            FilePath = relativePath,
            UploadedAt = DateTime.UtcNow,
            DownloadsCount = 0,
            AverageRating = null
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);

        var systemModeratorId = await GetSystemModeratorIdAsync();
        if (systemModeratorId.HasValue)
        {
            _context.ModerationLogs.Add(new ModerationLog
            {
                NoteId = note.Id,
                ModeratorId = systemModeratorId.Value,
                Action = actionCode,
                Comment = systemComment,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync(cancellationToken);
        }

        return CreatedAtAction(nameof(GetNote), new { id = note.Id }, new
        {
            note.Id,
            note.Title,
            note.UploadedAt,
            StatusId = note.StatusId,
            Message = responseMessage
        });
    }

    private async Task<Subject?> ResolveSubjectAsync(User user, int subjectId, string? subjectName, CancellationToken cancellationToken)
    {
        var subjects = await _context.Subjects
            .Where(s => s.UniversityId == user.UniversityId)
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);

        var subjectById = subjects.FirstOrDefault(s => s.Id == subjectId);
        if (subjectById != null)
        {
            return subjectById;
        }

        var normalizedSubjectName = NormalizeReferenceValue(subjectName);
        if (string.IsNullOrWhiteSpace(normalizedSubjectName))
        {
            return null;
        }

        return subjects.FirstOrDefault(s => NormalizeReferenceValue(s.Name) == normalizedSubjectName);
    }

    private async Task<Teacher?> ResolveTeacherAsync(User user, int? teacherId, string? teacherName, CancellationToken cancellationToken)
    {
        if (!teacherId.HasValue && string.IsNullOrWhiteSpace(teacherName))
        {
            return null;
        }

        var teachers = await _context.Teachers
            .Where(t => t.UniversityId == user.UniversityId)
            .OrderBy(t => t.Id)
            .ToListAsync(cancellationToken);

        var teacherById = teacherId.HasValue
            ? teachers.FirstOrDefault(t => t.Id == teacherId.Value)
            : null;
        if (teacherById != null)
        {
            return teacherById;
        }

        var normalizedTeacherName = NormalizeReferenceValue(teacherName);
        if (string.IsNullOrWhiteSpace(normalizedTeacherName))
        {
            return null;
        }

        return teachers.FirstOrDefault(t => NormalizeReferenceValue(t.FullName) == normalizedTeacherName);
    }

    private static string NormalizeReferenceValue(string? value)
    {
        return string.Join(' ', (value ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToLowerInvariant();
    }

    [HttpPost("moderate/{noteId:int}")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> ModerateNote(int noteId, [FromBody] ModerateModel model)
    {
        var note = await _context.Notes.FindAsync(noteId);
        if (note == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        var newStatus = await _context.NoteStatuses.FindAsync(model.StatusId);
        if (newStatus == null)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Статус не существует");
        }

        note.StatusId = model.StatusId;
        await _context.SaveChangesAsync();

        var moderatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        _context.ModerationLogs.Add(new ModerationLog
        {
            NoteId = noteId,
            ModeratorId = moderatorId,
            Action = newStatus.Code,
            Comment = model.Comment,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Статус конспекта изменён на {newStatus.Name}" });
    }

    [HttpGet("moderation/pending")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> GetModerationQueue()
    {
        var pendingStatusId = await GetStatusIdAsync("pending");
        if (!pendingStatusId.HasValue)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Не найден статус pending");
        }

        var queue = await _context.Notes
            .Where(n => n.StatusId == pendingStatusId.Value)
            .Include(n => n.Subject)
            .Include(n => n.University)
            .Include(n => n.User)
            .OrderByDescending(n => n.UploadedAt)
            .Select(n => new
            {
                n.Id,
                n.Title,
                Subject = n.Subject != null ? n.Subject.Name : "",
                University = n.University != null ? n.University.Name : "",
                Author = n.User != null ? n.User.FullName : "",
                n.UploadedAt
            })
            .ToListAsync();

        return Ok(queue);
    }

    [HttpGet("moderation/history")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> GetModerationHistory()
    {
        var historyActions = new[] { "approved", "rejected", "auto_approved", "auto_rejected" };

        var history = await _context.ModerationLogs
            .Where(l => historyActions.Contains(l.Action))
            .Include(l => l.Note)
                .ThenInclude(n => n!.Subject)
            .Include(l => l.Note)
                .ThenInclude(n => n!.University)
            .Include(l => l.Note)
                .ThenInclude(n => n!.User)
            .Include(l => l.Moderator)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new
            {
                l.Id,
                l.NoteId,
                NoteTitle = l.Note != null ? l.Note.Title : "",
                Subject = l.Note != null && l.Note.Subject != null ? l.Note.Subject.Name : "",
                University = l.Note != null && l.Note.University != null ? l.Note.University.Name : "",
                Author = l.Note != null && l.Note.User != null ? l.Note.User.FullName : "",
                UploadedAt = l.Note != null ? l.Note.UploadedAt : (DateTime?)null,
                ResultCode = l.Action == "approved" || l.Action == "auto_approved" ? "approved" : "rejected",
                l.Action,
                l.Comment,
                l.CreatedAt,
                Moderator = l.Moderator != null ? l.Moderator.FullName : ""
            })
            .ToListAsync();

        return Ok(history);
    }

    [HttpGet("all")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> GetAllNotes([FromQuery] int? statusId = null)
    {
        var query = _context.Notes
            .Include(n => n.User)
            .Include(n => n.Status)
            .AsQueryable();

        if (statusId.HasValue)
        {
            query = query.Where(n => n.StatusId == statusId.Value);
        }

        var notes = await query
            .OrderByDescending(n => n.UploadedAt)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.UploadedAt,
                UserEmail = n.User != null ? n.User.Email : "",
                Status = n.Status != null ? n.Status.Name : "",
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
            .Include(n => n.Status)
            .Where(n => n.Status != null && n.Status.Code == "approved");

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
                n.SubjectId,
                n.TeacherId,
                n.UniversityId,
                Subject = n.Subject != null ? n.Subject.Name : "",
                Teacher = n.Teacher != null ? n.Teacher.FullName : "",
                University = n.University != null ? n.University.Name : "",
                Rating = n.AverageRating ?? 0,
                DownloadsCount = n.DownloadsCount
            })
            .ToListAsync();

        return Ok(notes);
    }

    [HttpGet("{id:int}")]
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
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        var isApproved = await IsStatusCodeAsync(note.StatusId, "approved");
        if (!isApproved)
        {
            var isAuthenticated = User?.Identity?.IsAuthenticated == true;
            if (!isAuthenticated || !await CanUserAccessNoteFileAsync(note))
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
            }
        }

        var currentUserRating = 0;
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out var currentUserId))
            {
                currentUserRating = await _context.NoteRatings
                    .Where(r => r.NoteId == note.Id && r.UserId == currentUserId)
                    .Select(r => r.Rating)
                    .FirstOrDefaultAsync();
            }
        }

        return Ok(new
        {
            note.Id,
            note.Title,
            note.Description,
            note.SubjectId,
            note.TeacherId,
            note.UniversityId,
            Subject = note.Subject?.Name ?? "",
            Teacher = note.Teacher?.FullName ?? "",
            University = note.University?.Name ?? "",
            note.FilePath,
            note.DownloadsCount,
            Rating = note.AverageRating ?? 0,
            UserRating = currentUserRating,
            Author = note.User?.FullName,
            UploadedAt = note.UploadedAt
        });
    }

    [HttpGet("{id:int}/file")]
    [Authorize]
    public async Task<IActionResult> ViewNoteFile(int id)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);
        if (note == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        if (!await CanUserAccessNoteFileAsync(note))
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, title: "Доступ запрещён");
        }

        var resolvedPath = ResolveNoteFilePath(note);
        if (resolvedPath == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Файл конспекта не найден");
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(resolvedPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var stream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(stream, contentType, enableRangeProcessing: true);
    }

    [HttpGet("{id:int}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadNoteFile(int id)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);
        if (note == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        if (!await CanUserAccessNoteFileAsync(note))
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, title: "Доступ запрещён");
        }

        var resolvedPath = ResolveNoteFilePath(note);
        if (resolvedPath == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Файл конспекта не найден");
        }

        var shouldCountDownload = !User.IsInRole("moderator") && !User.IsInRole("admin");
        if (shouldCountDownload)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            note.DownloadsCount += 1;
            _context.NoteDownloads.Add(new NoteDownload
            {
                NoteId = note.Id,
                UserId = userId,
                DownloadedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(resolvedPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var downloadName = ExtractStoredFileName(note.FilePath);
        if (string.IsNullOrWhiteSpace(downloadName))
        {
            downloadName = $"note-{note.Id}.pdf";
        }

        var stream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(stream, contentType, downloadName);
    }

    [HttpPost("{id:int}/rate")]
    [Authorize]
    public async Task<IActionResult> RateNote(int id, [FromBody] RateNoteModel model)
    {
        if (model.Rating < 1 || model.Rating > 10)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Оценка должна быть в диапазоне от 1 до 10");
        }

        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);
        if (note == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "РљРѕРЅСЃРїРµРєС‚ РЅРµ РЅР°Р№РґРµРЅ");
        }

        if (!await CanUserAccessNoteFileAsync(note))
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, title: "Р”РѕСЃС‚СѓРї Р·Р°РїСЂРµС‰С‘РЅ");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var existingRating = await _context.NoteRatings
            .FirstOrDefaultAsync(r => r.NoteId == id && r.UserId == userId);

        if (existingRating == null)
        {
            _context.NoteRatings.Add(new NoteRating
            {
                NoteId = id,
                UserId = userId,
                Rating = model.Rating,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existingRating.Rating = model.Rating;
            existingRating.UpdatedAt = DateTime.UtcNow;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.NumericValueOutOfRange)
        {
            await _context.Database.ExecuteSqlRawAsync("""
                ALTER TABLE notes
                ALTER COLUMN average_rating TYPE numeric(4,2)
                USING average_rating::numeric(4,2);
                """);

            await _context.SaveChangesAsync();
        }

        var averageRating = await _context.NoteRatings
            .Where(r => r.NoteId == id)
            .AverageAsync(r => (double?)r.Rating);

        note.AverageRating = averageRating.HasValue
            ? Math.Round((decimal)averageRating.Value, 1, MidpointRounding.AwayFromZero)
            : null;
        await _context.SaveChangesAsync();

        var votesCount = await _context.NoteRatings.CountAsync(r => r.NoteId == id);

        return Ok(new
        {
            note.Id,
            Rating = note.AverageRating ?? 0,
            UserRating = model.Rating,
            VotesCount = votesCount
        });
    }

    [HttpPost("{id:int}/complaints")]
    [Authorize]
    public async Task<IActionResult> CreateComplaint(int id, [FromBody] CreateComplaintModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var note = await _context.Notes.FindAsync(id);
        if (note == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        var isApproved = await IsStatusCodeAsync(note.StatusId, "approved");
        if (!isApproved && note.UserId != userId && !User.IsInRole("admin") && !User.IsInRole("moderator"))
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        if (note.UserId == userId)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Нельзя отправить жалобу на собственный конспект");
        }

        if (string.IsNullOrWhiteSpace(model.Reason))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Укажите причину жалобы");
        }

        var alreadyExists = await _context.Complaints
            .AnyAsync(c => c.NoteId == id && c.ReporterId == userId && c.Status == "open");
        if (alreadyExists)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Жалоба уже отправлена", detail: "У вас уже есть открытая жалоба на этот конспект.");
        }

        var complaint = new Complaint
        {
            NoteId = id,
            ReporterId = userId,
            Reason = model.Reason.Trim(),
            Comment = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim(),
            Status = "open",
            CreatedAt = DateTime.UtcNow
        };

        _context.Complaints.Add(complaint);
        await _context.SaveChangesAsync();

        return Created($"/api/notes/complaints/{complaint.Id}", new
        {
            complaint.Id,
            complaint.NoteId,
            complaint.Reason,
            complaint.Status,
            complaint.CreatedAt,
            Message = "Жалоба отправлена модератору"
        });
    }

    [HttpGet("complaints/my")]
    [Authorize]
    public async Task<IActionResult> GetMyComplaints()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var complaints = await _context.Complaints
            .Where(c => c.ReporterId == userId)
            .Include(c => c.Note)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.NoteId,
                NoteTitle = c.Note != null ? c.Note.Title : "",
                c.Reason,
                c.Comment,
                c.Status,
                c.CreatedAt,
                c.ResolvedAt
            })
            .ToListAsync();

        return Ok(complaints);
    }

    [HttpGet("complaints")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> GetComplaints([FromQuery] string? status = "open")
    {
        var query = _context.Complaints
            .Include(c => c.Note)
                .ThenInclude(n => n!.User)
            .Include(c => c.Reporter)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(c => c.Status == status);
        }

        var complaints = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.NoteId,
                NoteTitle = c.Note != null ? c.Note.Title : "",
                Author = c.Note != null && c.Note.User != null ? c.Note.User.FullName : "",
                Reporter = c.Reporter != null ? c.Reporter.FullName : "",
                c.Reason,
                c.Comment,
                c.Status,
                c.CreatedAt
            })
            .ToListAsync();

        return Ok(complaints);
    }

    [HttpGet("complaints/{complaintId:int}")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> GetComplaintDetails(int complaintId)
    {
        var complaint = await _context.Complaints
            .Include(c => c.Note)
                .ThenInclude(n => n!.User)
            .Include(c => c.Reporter)
            .Include(c => c.ResolvedBy)
            .FirstOrDefaultAsync(c => c.Id == complaintId);

        if (complaint == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Жалоба не найдена");
        }

        return Ok(new
        {
            complaint.Id,
            complaint.NoteId,
            NoteTitle = complaint.Note != null ? complaint.Note.Title : "",
            NoteDescription = complaint.Note != null ? complaint.Note.Description : "",
            complaint.Reason,
            complaint.Comment,
            complaint.Status,
            complaint.CreatedAt,
            complaint.ResolvedAt,
            complaint.ResolutionComment,
            Reporter = complaint.Reporter != null ? complaint.Reporter.FullName : "",
            Author = complaint.Note != null && complaint.Note.User != null ? complaint.Note.User.FullName : "",
            ResolvedBy = complaint.ResolvedBy != null ? complaint.ResolvedBy.FullName : ""
        });
    }

    [HttpPost("complaints/{complaintId:int}/resolve")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> ResolveComplaint(int complaintId, [FromBody] ResolveComplaintModel model)
    {
        var complaint = await _context.Complaints
            .Include(c => c.Note)
            .FirstOrDefaultAsync(c => c.Id == complaintId);

        if (complaint == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Жалоба не найдена");
        }

        if (complaint.Status != "open")
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Жалоба уже обработана");
        }

        var moderatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        complaint.ResolvedAt = DateTime.UtcNow;
        complaint.ResolvedById = moderatorId;
        complaint.ResolutionComment = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim();

        if (model.ConfirmComplaint)
        {
            var pendingStatusId = await GetStatusIdAsync("pending");
            if (!pendingStatusId.HasValue)
            {
                return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Не найден статус pending");
            }

            complaint.Status = "confirmed";
            if (complaint.Note != null)
            {
                complaint.Note.StatusId = pendingStatusId.Value;
            }

            _context.ModerationLogs.Add(new ModerationLog
            {
                NoteId = complaint.NoteId,
                ModeratorId = moderatorId,
                Action = "complaint_confirmed",
                Comment = complaint.ResolutionComment ?? "Жалоба подтверждена. Конспект отправлен на повторную проверку.",
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            complaint.Status = "dismissed";
            _context.ModerationLogs.Add(new ModerationLog
            {
                NoteId = complaint.NoteId,
                ModeratorId = moderatorId,
                Action = "complaint_dismissed",
                Comment = complaint.ResolutionComment ?? "Жалоба отклонена.",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            complaint.Id,
            complaint.Status,
            complaint.ResolvedAt,
            Message = model.ConfirmComplaint
                ? "Жалоба подтверждена, конспект отправлен на повторную проверку"
                : "Жалоба отклонена"
        });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "student,admin")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteModel model)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (note.UserId != userId && userRole != "admin")
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, title: "Доступ запрещён", detail: "Вы можете редактировать только свои конспекты.");
        }

        if (!string.IsNullOrWhiteSpace(model.Title))
        {
            note.Title = model.Title;
        }

        if (model.Description != null)
        {
            note.Description = model.Description;
        }

        if (model.SubjectId.HasValue)
        {
            var subject = await _context.Subjects.FindAsync(model.SubjectId.Value);
            if (subject == null)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Предмет не найден");
            }

            if (subject.UniversityId != note.UniversityId)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "РџСЂРµРґРјРµС‚ РЅРµ РїСЂРёРЅР°РґР»РµР¶РёС‚ РІС‹Р±СЂР°РЅРЅРѕРјСѓ РІСѓР·Сѓ");
            }

            note.SubjectId = model.SubjectId.Value;
        }

        if (model.TeacherId.HasValue)
        {
            var teacher = await _context.Teachers.FindAsync(model.TeacherId.Value);
            if (teacher == null)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Преподаватель не найден");
            }

            if (teacher.UniversityId != note.UniversityId)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "РџСЂРµРїРѕРґР°РІР°С‚РµР»СЊ РЅРµ РїСЂРёРЅР°РґР»РµР¶РёС‚ РІС‹Р±СЂР°РЅРЅРѕРјСѓ РІСѓР·Сѓ");
            }

            note.TeacherId = model.TeacherId.Value;
        }

        if (await IsStatusCodeAsync(note.StatusId, "approved"))
        {
            var pendingStatusId = await GetStatusIdAsync("pending");
            if (pendingStatusId.HasValue)
            {
                note.StatusId = pendingStatusId.Value;
            }
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

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "student,admin")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (note.UserId != userId && userRole != "admin")
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, title: "Доступ запрещён", detail: "Вы можете удалять только свои конспекты.");
        }

        if (!string.IsNullOrEmpty(note.FilePath))
        {
            var filePath = ResolveNoteFilePath(note);
            if (!string.IsNullOrWhiteSpace(filePath) && System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Конспект успешно удалён" });
    }

    [HttpPost("{id:int}/favorite")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleFavorite(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var note = await _context.Notes.FindAsync(id);
        if (note == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден");
        }

        var existingFavorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.NoteId == id);

        if (existingFavorite != null)
        {
            _context.Favorites.Remove(existingFavorite);
            await _context.SaveChangesAsync();
            return Ok(new { isFavorite = false, message = "Удалено из избранного" });
        }

        _context.Favorites.Add(new Favorite
        {
            UserId = userId,
            NoteId = id,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return Created($"/api/notes/{id}/favorite", new { isFavorite = true, message = "Добавлено в избранное" });
    }

    [HttpDelete("{id:int}/favorite")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFavorite(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var existingFavorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.NoteId == id);

        if (existingFavorite == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Конспект не найден в избранном");
        }

        _context.Favorites.Remove(existingFavorite);
        await _context.SaveChangesAsync();

        return Ok(new { isFavorite = false, message = "Удалено из избранного" });
    }

    [HttpGet("favorites")]
    [Authorize]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Note!)
                .ThenInclude(n => n.University)
            .Include(f => f.Note!)
                .ThenInclude(n => n.Subject)
            .Include(f => f.Note!)
                .ThenInclude(n => n.Teacher)
            .Include(f => f.Note!)
                .ThenInclude(n => n.Status)
            .Where(f => f.Note != null)
            .Select(f => new
            {
                f.Note!.Id,
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
