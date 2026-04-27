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
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseBootstrapper");

        await EnsureComplaintsTableAsync(context, cancellationToken);
        await EnsureNoteRatingsTableAsync(context, cancellationToken);
        await EnsureNoteDownloadsTableAsync(context, cancellationToken);
        await EnsureRolesAsync(context, cancellationToken);
        await EnsureNoteStatusesAsync(context, cancellationToken);
        await EnsureNotesRatingColumnAsync(context, cancellationToken);
        await EnsureTxtNoteFilesConvertedAsync(context, env, logger, cancellationToken);
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

    private static async Task EnsureNoteRatingsTableAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS note_ratings
            (
                id SERIAL PRIMARY KEY,
                note_id INTEGER NOT NULL REFERENCES notes(id) ON DELETE CASCADE,
                user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                rating INTEGER NOT NULL CHECK (rating BETWEEN 1 AND 10),
                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );
            CREATE UNIQUE INDEX IF NOT EXISTS ux_note_ratings_note_user ON note_ratings(note_id, user_id);
            CREATE INDEX IF NOT EXISTS ix_note_ratings_note_id ON note_ratings(note_id);
            """;

        await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }

    private static async Task EnsureNoteDownloadsTableAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS note_downloads
            (
                id SERIAL PRIMARY KEY,
                note_id INTEGER NOT NULL REFERENCES notes(id) ON DELETE CASCADE,
                user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                downloaded_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );
            CREATE INDEX IF NOT EXISTS ix_note_downloads_user_id ON note_downloads(user_id);
            CREATE INDEX IF NOT EXISTS ix_note_downloads_note_id ON note_downloads(note_id);
            CREATE INDEX IF NOT EXISTS ix_note_downloads_downloaded_at ON note_downloads(downloaded_at);
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

    private static async Task EnsureNotesRatingColumnAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        const string sql = """
            ALTER TABLE notes
            ALTER COLUMN average_rating TYPE numeric(4,2)
            USING average_rating::numeric(4,2);
            """;

        await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }

    private static async Task EnsureTxtNoteFilesConvertedAsync(
        ApplicationDbContext context,
        IWebHostEnvironment env,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var txtNotes = await context.Notes
            .Where(n => n.FilePath.EndsWith(".txt"))
            .ToListAsync(cancellationToken);

        if (txtNotes.Count == 0)
        {
            return;
        }

        var contentRootUploads = Path.Combine(env.ContentRootPath, "uploads");
        var webRootUploads = string.IsNullOrWhiteSpace(env.WebRootPath)
            ? null
            : Path.Combine(env.WebRootPath, "uploads");

        var updatedCount = 0;

        foreach (var note in txtNotes)
        {
            var fileName = Path.GetFileName(note.FilePath.Replace('\\', '/'));
            if (string.IsNullOrWhiteSpace(fileName))
            {
                continue;
            }

            var txtCandidates = new List<string>
            {
                Path.Combine(contentRootUploads, fileName)
            };

            if (!string.IsNullOrWhiteSpace(webRootUploads))
            {
                txtCandidates.Add(Path.Combine(webRootUploads, fileName));
            }

            var txtPath = txtCandidates.FirstOrDefault(File.Exists);
            if (txtPath == null)
            {
                continue;
            }

            var pdfPath = Path.ChangeExtension(txtPath, ".pdf")!;
            if (!File.Exists(pdfPath))
            {
                var txtContent = await File.ReadAllTextAsync(txtPath, cancellationToken);
                var pdfBytes = BuildSimplePdfFromText(string.IsNullOrWhiteSpace(txtContent)
                    ? "StudyNotes placeholder PDF"
                    : txtContent);
                await File.WriteAllBytesAsync(pdfPath, pdfBytes, cancellationToken);
            }

            note.FilePath = Path.ChangeExtension(note.FilePath.Replace('\\', '/'), ".pdf")!;
            updatedCount++;
        }

        if (updatedCount > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Converted {Count} txt note files to pdf placeholders.", updatedCount);
        }
    }

    private static byte[] BuildSimplePdfFromText(string text)
    {
        static string EscapePdfText(string value)
        {
            return value
                .Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("(", "\\(", StringComparison.Ordinal)
                .Replace(")", "\\)", StringComparison.Ordinal);
        }

        var lines = text
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .Take(40)
            .Select(l => l.Length > 80 ? l[..80] : l)
            .ToList();

        if (lines.Count == 0)
        {
            lines.Add("StudyNotes PDF placeholder");
        }

        var content = new StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 12 Tf");
        content.AppendLine("50 780 Td");
        for (var i = 0; i < lines.Count; i++)
        {
            if (i > 0)
            {
                content.AppendLine("0 -16 Td");
            }
            content.Append('(').Append(EscapePdfText(lines[i])).AppendLine(") Tj");
        }
        content.AppendLine("ET");

        var contentBytes = Encoding.ASCII.GetBytes(content.ToString());

        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.ASCII, leaveOpen: true);

        writer.WriteLine("%PDF-1.4");
        writer.Flush();

        var offsets = new List<long>();

        offsets.Add(ms.Position);
        writer.WriteLine("1 0 obj");
        writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
        writer.WriteLine("endobj");
        writer.Flush();

        offsets.Add(ms.Position);
        writer.WriteLine("2 0 obj");
        writer.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
        writer.WriteLine("endobj");
        writer.Flush();

        offsets.Add(ms.Position);
        writer.WriteLine("3 0 obj");
        writer.WriteLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>");
        writer.WriteLine("endobj");
        writer.Flush();

        offsets.Add(ms.Position);
        writer.WriteLine("4 0 obj");
        writer.WriteLine($"<< /Length {contentBytes.Length} >>");
        writer.WriteLine("stream");
        writer.Flush();
        ms.Write(contentBytes, 0, contentBytes.Length);
        writer.WriteLine();
        writer.WriteLine("endstream");
        writer.WriteLine("endobj");
        writer.Flush();

        offsets.Add(ms.Position);
        writer.WriteLine("5 0 obj");
        writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
        writer.WriteLine("endobj");
        writer.Flush();

        var xrefStart = ms.Position;
        writer.WriteLine("xref");
        writer.WriteLine("0 6");
        writer.WriteLine("0000000000 65535 f ");
        foreach (var offset in offsets)
        {
            writer.WriteLine($"{offset:D10} 00000 n ");
        }

        writer.WriteLine("trailer");
        writer.WriteLine("<< /Size 6 /Root 1 0 R >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xrefStart);
        writer.WriteLine("%%EOF");
        writer.Flush();

        return ms.ToArray();
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
