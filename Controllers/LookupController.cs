using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Data;
using System.Security.Claims;

namespace StudyNotesPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private static readonly string[] AllowedUniversities = ["ПГГПУ", "ПГНИУ", "СПбГУ"];
    private static readonly Dictionary<string, string[]> AllowedSubjectsByUniversity = new(StringComparer.Ordinal)
    {
        ["ПГГПУ"] = ["Педагогика", "Психология образования", "Методика преподавания математики", "История России", "Философия"],
        ["ПГНИУ"] = ["Программирование", "Базы данных", "Дискретная математика", "Операционные системы", "Философия"],
        ["СПбГУ"] = ["Математический анализ", "Теория вероятностей", "Алгоритмы и структуры данных", "Философия"]
    };
    private static readonly Dictionary<string, string[]> AllowedTeachersByUniversity = new(StringComparer.Ordinal)
    {
        ["ПГГПУ"] = ["Соколова Анна Викторовна", "Кузнецов Илья Сергеевич", "Морозова Елена Павловна"],
        ["ПГНИУ"] = ["Иванов Дмитрий Алексеевич", "Петрова Марина Олеговна", "Смирнов Кирилл Андреевич", "Кузнецов Илья Сергеевич", "Морозова Елена Павловна"],
        ["СПбГУ"] = ["Васильева Наталья Игоревна", "Орлов Максим Петрович", "Андреева Софья Романовна"]
    };

    public LookupController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/lookup/subjects (для своего университета)
    [HttpGet("subjects")]
    [Authorize]
    public async Task<IActionResult> GetSubjects()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users
            .Include(u => u.University)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Требуется авторизация", detail: "Пользователь из токена не найден.");

        if (user.University == null || !AllowedSubjectsByUniversity.TryGetValue(user.University.Name, out var allowedSubjects))
        {
            return Ok(Array.Empty<object>());
        }

        var allowedSubjectNames = allowedSubjects
            .Select(NormalizeLookupValue)
            .ToHashSet(StringComparer.Ordinal);

        var subjects = (await _context.Subjects
            .Where(s => s.UniversityId == user.UniversityId)
            .OrderBy(s => s.Id)
            .Select(s => new { s.Id, s.Name })
            .ToListAsync())
            .Where(s => allowedSubjectNames.Contains(NormalizeLookupValue(s.Name)))
            .GroupBy(s => NormalizeLookupValue(s.Name))
            .Select(g => g.First())
            .OrderBy(s => s.Name)
            .ToList();
        return Ok(subjects);
    }

    // GET: api/lookup/teachers (для своего университета)
    [HttpGet("teachers")]
    [Authorize]
    public async Task<IActionResult> GetTeachers()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users
            .Include(u => u.University)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Требуется авторизация", detail: "Пользователь из токена не найден.");

        if (user.University == null || !AllowedTeachersByUniversity.TryGetValue(user.University.Name, out var allowedTeachers))
        {
            return Ok(Array.Empty<object>());
        }

        var allowedTeacherNames = allowedTeachers
            .Select(NormalizeLookupValue)
            .ToHashSet(StringComparer.Ordinal);

        var teachers = (await _context.Teachers
            .Where(t => t.UniversityId == user.UniversityId)
            .OrderBy(t => t.Id)
            .Select(t => new { t.Id, t.FullName })
            .ToListAsync())
            .Where(t => allowedTeacherNames.Contains(NormalizeLookupValue(t.FullName)))
            .GroupBy(t => NormalizeLookupValue(t.FullName))
            .Select(g => g.First())
            .OrderBy(t => t.FullName)
            .ToList();
        return Ok(teachers);
    }

    // НОВЫЙ: GET: api/lookup/all-universities (для фильтров)
    [HttpGet("all-universities")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUniversities()
    {
        var universities = await _context.Universities
            .Where(u => AllowedUniversities.Contains(u.Name))
            .OrderBy(u => u.Name)
            .Select(u => new { u.Id, u.Name })
            .ToListAsync();
        return Ok(universities);
    }

    // НОВЫЙ: GET: api/lookup/all-subjects (для фильтров)
    [HttpGet("all-subjects")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSubjects([FromQuery] int? universityId = null)
    {
        var query = _context.Subjects
            .Where(s => AllowedUniversities.Contains(s.University!.Name))
            .Select(s => new { s.Id, s.Name, s.UniversityId, UniversityName = s.University!.Name })
            .AsQueryable();
        if (universityId.HasValue)
        {
            query = query.Where(s => s.UniversityId == universityId.Value);
        }

        var subjects = (await query
            .OrderBy(s => s.Name)
            .ToListAsync())
            .Where(s => AllowedSubjectsByUniversity.TryGetValue(s.UniversityName, out var allowedSubjects)
                && allowedSubjects.Select(NormalizeLookupValue).Contains(NormalizeLookupValue(s.Name)))
            .GroupBy(s => new { s.UniversityId, Key = NormalizeLookupValue(s.Name) })
            .Select(g => new { g.First().Id, g.First().Name, g.First().UniversityId })
            .OrderBy(s => s.Name)
            .ToList();
        return Ok(subjects);
    }

    // НОВЫЙ: GET: api/lookup/all-teachers (для фильтров)
    [HttpGet("all-teachers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllTeachers([FromQuery] int? universityId = null)
    {
        var query = _context.Teachers
            .Where(t => AllowedUniversities.Contains(t.University!.Name))
            .Select(t => new { t.Id, t.FullName, t.UniversityId, UniversityName = t.University!.Name })
            .AsQueryable();
        if (universityId.HasValue)
        {
            query = query.Where(t => t.UniversityId == universityId.Value);
        }

        var teachers = (await query
            .OrderBy(t => t.FullName)
            .ToListAsync())
            .Where(t => AllowedTeachersByUniversity.TryGetValue(t.UniversityName, out var allowedTeachers)
                && allowedTeachers.Select(NormalizeLookupValue).Contains(NormalizeLookupValue(t.FullName)))
            .GroupBy(t => new { t.UniversityId, Key = NormalizeLookupValue(t.FullName) })
            .Select(g => new { g.First().Id, g.First().FullName, g.First().UniversityId })
            .OrderBy(t => t.FullName)
            .ToList();
        return Ok(teachers);
    }

    private static string NormalizeLookupValue(string? value)
    {
        return string.Join(' ', (value ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToLowerInvariant();
    }
}
