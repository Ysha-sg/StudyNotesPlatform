using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudyNotesPlatform.Controllers;

// Модель для обновления профиля
public class UpdateProfileModel
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public int? UniversityId { get; set; }
    public string? NewPassword { get; set; }
}

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

    // GET: api/profile/me
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

    // PUT: api/profile/update
    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound(new { message = "Пользователь не найден" });

        // Обновляем имя
        if (!string.IsNullOrWhiteSpace(model.FullName))
            user.FullName = model.FullName;

        // Обновляем email (проверяем уникальность)
        if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != user.Email)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
                return BadRequest(new { message = "Пользователь с таким email уже существует" });

            user.Email = model.Email;
        }

        // Обновляем университет
        if (model.UniversityId.HasValue)
        {
            var university = await _context.Universities
                .FindAsync(model.UniversityId.Value);

            if (university == null)
                return BadRequest(new { message = "Университет не найден" });

            user.UniversityId = model.UniversityId.Value;
        }

        // Обновляем пароль (если указан)
        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            if (model.NewPassword.Length < 6)
                return BadRequest(new { message = "Пароль должен быть не менее 6 символов" });

            user.PasswordHash = HashPassword(model.NewPassword);
        }

        await _context.SaveChangesAsync();

        // Возвращаем обновлённые данные
        var updatedUser = await _context.Users
            .Include(u => u.University)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return Ok(new
        {
            message = "Профиль успешно обновлён",
            fullName = updatedUser?.FullName,
            email = updatedUser?.Email,
            university = updatedUser?.University?.Name
        });
    }

    // GET: api/profile/admin-only (пример метода только для админа)
    [HttpGet("admin-only")]
    [Authorize(Roles = "admin")]
    public IActionResult AdminZone()
    {
        return Ok("Только для администратора");
    }

    // Приватный метод для хэширования пароля
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}