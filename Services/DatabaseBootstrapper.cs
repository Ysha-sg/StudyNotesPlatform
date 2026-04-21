using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Data;
using StudyNotesPlatform.Models;
using System.Security.Cryptography;
using System.Text;

namespace StudyNotesPlatform.Services;

public static class DatabaseBootstrapper
{
    public const string DefaultModeratorEmail = "mod@mod.ru";
    public const string DefaultModeratorPassword = "123456";
    public const string DefaultModeratorName = "Модератор";

    public static async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseBootstrapper");

        await EnsureComplaintsTableAsync(context, cancellationToken);
        await EnsureRolesAsync(context, cancellationToken);
        await EnsureNoteStatusesAsync(context, cancellationToken);
        await EnsureDefaultModeratorAsync(context, logger, cancellationToken);

        logger.LogInformation("Database bootstrap complete.");
    }

    private static async Task EnsureComplaintsTableAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS complaints
            (
                id SERIAL PRIMARY KEY,
                note_id INTEGER NOT NULL REFERENCES notes(id) ON DELETE CASCADE,
                reporter_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                reason TEXT NOT NULL,
                comment TEXT NULL,
                status VARCHAR(32) NOT NULL DEFAULT 'open',
                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                resolved_at TIMESTAMPTZ NULL,
                resolved_by_id INTEGER NULL REFERENCES users(id) ON DELETE SET NULL,
                resolution_comment TEXT NULL
            );
            CREATE INDEX IF NOT EXISTS ix_complaints_note_id ON complaints(note_id);
            CREATE INDEX IF NOT EXISTS ix_complaints_status ON complaints(status);
            CREATE INDEX IF NOT EXISTS ix_complaints_note_reporter_status ON complaints(note_id, reporter_id, status);
            """;

        await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }

    private static async Task EnsureRolesAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var defaults = new[]
        {
            new Role { Code = "student", Name = "Студент" },
            new Role { Code = "moderator", Name = "Модератор" },
            new Role { Code = "admin", Name = "Администратор" }
        };

        foreach (var role in defaults)
        {
            var existingRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == role.Code, cancellationToken);
            if (existingRole == null)
            {
                context.Roles.Add(role);
                continue;
            }

            if (!string.Equals(existingRole.Name, role.Name, StringComparison.Ordinal))
            {
                existingRole.Name = role.Name;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureNoteStatusesAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var defaults = new[]
        {
            new NoteStatus { Code = "pending", Name = "На проверке", Description = "Ожидает модерации" },
            new NoteStatus { Code = "approved", Name = "Одобрено", Description = "Опубликовано в каталоге" },
            new NoteStatus { Code = "rejected", Name = "Отклонено", Description = "Отклонено модератором или автопроверкой" }
        };

        foreach (var status in defaults)
        {
            var existingStatus = await context.NoteStatuses.FirstOrDefaultAsync(s => s.Code == status.Code, cancellationToken);
            if (existingStatus == null)
            {
                context.NoteStatuses.Add(status);
                continue;
            }

            if (!string.Equals(existingStatus.Name, status.Name, StringComparison.Ordinal))
            {
                existingStatus.Name = status.Name;
            }

            if (!string.Equals(existingStatus.Description, status.Description, StringComparison.Ordinal))
            {
                existingStatus.Description = status.Description;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureDefaultModeratorAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var moderatorRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "moderator", cancellationToken);
        if (moderatorRole == null)
        {
            logger.LogWarning("Role 'moderator' was not found while seeding default moderator.");
            return;
        }

        var university = await context.Universities.OrderBy(u => u.Id).FirstOrDefaultAsync(cancellationToken);
        if (university == null)
        {
            university = new University
            {
                Name = "Тестовый университет",
                City = "Пермь"
            };
            context.Universities.Add(university);
            await context.SaveChangesAsync(cancellationToken);
        }

        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == DefaultModeratorEmail, cancellationToken);
        if (existingUser == null)
        {
            var newUser = new User
            {
                RoleId = moderatorRole.Id,
                UniversityId = university.Id,
                FullName = DefaultModeratorName,
                Email = DefaultModeratorEmail,
                PasswordHash = HashPassword(DefaultModeratorPassword),
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Default moderator user created with email {Email}.", DefaultModeratorEmail);
            return;
        }

        var targetHash = HashPassword(DefaultModeratorPassword);
        var updated = false;

        if (existingUser.RoleId != moderatorRole.Id)
        {
            existingUser.RoleId = moderatorRole.Id;
            updated = true;
        }

        if (!string.Equals(existingUser.PasswordHash, targetHash, StringComparison.Ordinal))
        {
            existingUser.PasswordHash = targetHash;
            updated = true;
        }

        if (!string.Equals(existingUser.FullName, DefaultModeratorName, StringComparison.Ordinal))
        {
            existingUser.FullName = DefaultModeratorName;
            updated = true;
        }

        if (updated)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Existing user {Email} updated as default moderator.", DefaultModeratorEmail);
        }
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
