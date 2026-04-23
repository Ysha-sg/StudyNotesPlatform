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
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Требуется авторизация", detail: "Пользователь из токена не найден.");

        var subjects = await _context.Subjects
            .Where(s => s.UniversityId == user.UniversityId)
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();
        return Ok(subjects);
    }

    // GET: api/lookup/teachers (для своего университета)
    [HttpGet("teachers")]
    [Authorize]
    public async Task<IActionResult> GetTeachers()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Требуется авторизация", detail: "Пользователь из токена не найден.");

        var teachers = await _context.Teachers
            .Where(t => t.UniversityId == user.UniversityId)
            .Select(t => new { t.Id, t.FullName })
            .ToListAsync();
        return Ok(teachers);
    }

    // НОВЫЙ: GET: api/lookup/all-universities (для фильтров)
    [HttpGet("all-universities")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUniversities()
    {
        var universities = await _context.Universities
            .Select(u => new { u.Id, u.Name })
            .ToListAsync();
        return Ok(universities);
    }

    // НОВЫЙ: GET: api/lookup/all-subjects (для фильтров)
    [HttpGet("all-subjects")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSubjects()
    {
        var subjects = await _context.Subjects
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();
        return Ok(subjects);
    }

    // НОВЫЙ: GET: api/lookup/all-teachers (для фильтров)
    [HttpGet("all-teachers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllTeachers()
    {
        var teachers = await _context.Teachers
            .Select(t => new { t.Id, t.FullName })
            .ToListAsync();
        return Ok(teachers);
    }
}
