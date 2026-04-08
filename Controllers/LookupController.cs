using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Data;
using System.Security.Claims;

namespace StudyNotesPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LookupController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LookupController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("subjects")]
    public async Task<IActionResult> GetSubjects()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Unauthorized();

        var subjects = await _context.Subjects
            .Where(s => s.UniversityId == user.UniversityId)
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();
        return Ok(subjects);
    }

    [HttpGet("teachers")]
    public async Task<IActionResult> GetTeachers()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Unauthorized();

        var teachers = await _context.Teachers
            .Where(t => t.UniversityId == user.UniversityId)
            .Select(t => new { t.Id, t.FullName })
            .ToListAsync();
        return Ok(teachers);
    }
}