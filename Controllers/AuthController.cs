using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Data;
using StudyNotesPlatform.Models;
using StudyNotesPlatform.Services;
using System.Security.Cryptography;
using System.Text;

namespace StudyNotesPlatform.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly ApplicationDbContext _context;

    public AuthController(TokenService tokenService, ApplicationDbContext context)
    {
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Models.RegisterModel model)
    {
        // 1. Проверка email
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (existingUser != null)
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Пользователь с таким email уже существует");

        // 2. Поиск университета (без учёта регистра)
        var university = await _context.Universities
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Name, model.UniversityName));
        if (university == null)
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: $"Университет '{model.UniversityName}' не найден");

        // 3. Поиск роли "student"
        var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Code == "student");
        if (studentRole == null)
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Роль 'student' не найдена");

        // 4. Хэширование пароля
        var passwordHash = HashPassword(model.Password);

        // 5. Создание пользователя
        var newUser = new User
        {
            RoleId = studentRole.Id,
            UniversityId = university.Id,
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var token = _tokenService.GenerateToken(newUser);

        return Created("/api/profile/me", new Models.AuthResponse
        {
            Token = token,
            FullName = newUser.FullName,
            Email = newUser.Email,
            UniversityName = university.Name,   // исправлено
            Message = "Регистрация успешна"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Models.LoginModel model)
    {
        var user = await _context.Users
            .Include(u => u.University)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Неверный email или пароль");
        }

        var token = _tokenService.GenerateToken(user);

        return Ok(new Models.AuthResponse
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email,
            UniversityName = user.University?.Name ?? string.Empty,   // исправлено
            Message = "Вход выполнен успешно"
        });
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}

