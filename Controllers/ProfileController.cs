using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Data;
using System.Security.Claims;

namespace StudyNotesPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // требует аутентификации для всех методов
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users
            .Include(u => u.University)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.FullName,
            user.Email,
            University = user.University?.Name,
            Role = user.Role?.Code
        });
    }

    [HttpGet("admin-only")]
    [Authorize(Roles = "admin")] // только админ
    public IActionResult AdminZone()
    {
        return Ok("Только для администратора");
    }
}