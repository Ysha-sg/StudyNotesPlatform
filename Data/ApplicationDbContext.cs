using Microsoft.EntityFrameworkCore;
using StudyNotesPlatform.Models;

namespace StudyNotesPlatform.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Существующие DbSet
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<ModerationLog> ModerationLogs { get; set; }
    public DbSet<NoteStatus> NoteStatuses { get; set; }

    // Новые DbSet для предметов и преподавателей
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Teacher> Teachers { get; set; }

    // DbSet для избранного
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<NoteRating> NoteRatings { get; set; }
    public DbSet<NoteDownload> NoteDownloads { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Role)
                  .WithMany()
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.University)
                  .WithMany()
                  .HasForeignKey(e => e.UniversityId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Roles
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Universities
        modelBuilder.Entity<University>(entity =>
        {
            entity.ToTable("universities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.City).HasColumnName("city");
        });

        // Notes
        modelBuilder.Entity<Note>(entity =>
        {
            entity.ToTable("notes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FilePath).HasColumnName("file_path");
            entity.Property(e => e.UploadedAt).HasColumnName("uploaded_at");
            entity.Property(e => e.DownloadsCount).HasColumnName("downloads_count");
            entity.Property(e => e.AverageRating).HasColumnName("average_rating");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.University)
                  .WithMany()
                  .HasForeignKey(e => e.UniversityId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Subject)
                  .WithMany()
                  .HasForeignKey(e => e.SubjectId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Teacher)
                  .WithMany()
                  .HasForeignKey(e => e.TeacherId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Status)
                  .WithMany()
                  .HasForeignKey(e => e.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ModerationLogs
        modelBuilder.Entity<ModerationLog>(entity =>
        {
            entity.ToTable("moderation_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NoteId).HasColumnName("note_id");
            entity.Property(e => e.ModeratorId).HasColumnName("moderator_id");
            entity.Property(e => e.Action).HasColumnName("action");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Note)
                  .WithMany()
                  .HasForeignKey(e => e.NoteId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Moderator)
                  .WithMany()
                  .HasForeignKey(e => e.ModeratorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // NoteStatuses
        modelBuilder.Entity<NoteStatus>(entity =>
        {
            entity.ToTable("note_statuses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Subjects
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("subjects");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.HasOne(e => e.University)
                  .WithMany()
                  .HasForeignKey(e => e.UniversityId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Teachers
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("teachers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.HasOne(e => e.University)
                  .WithMany()
                  .HasForeignKey(e => e.UniversityId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Favorites (избранное) — НОВЫЙ БЛОК
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("favorites");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("favorite_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.NoteId).HasColumnName("note_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Note)
                  .WithMany()
                  .HasForeignKey(e => e.NoteId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.NoteId }).IsUnique();
        });

        // Complaints (жалобы)
        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.ToTable("complaints");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NoteId).HasColumnName("note_id");
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");
            entity.Property(e => e.ResolvedById).HasColumnName("resolved_by_id");
            entity.Property(e => e.ResolutionComment).HasColumnName("resolution_comment");

            entity.HasOne(e => e.Note)
                .WithMany()
                .HasForeignKey(e => e.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Reporter)
                .WithMany()
                .HasForeignKey(e => e.ReporterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ResolvedBy)
                .WithMany()
                .HasForeignKey(e => e.ResolvedById)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.NoteId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.NoteId, e.ReporterId, e.Status });
        });

        // NoteRatings (оценки)
        modelBuilder.Entity<NoteRating>(entity =>
        {
            entity.ToTable("note_ratings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NoteId).HasColumnName("note_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.Note)
                .WithMany()
                .HasForeignKey(e => e.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.NoteId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.NoteId);
        });

        // NoteDownloads (история скачиваний)
        modelBuilder.Entity<NoteDownload>(entity =>
        {
            entity.ToTable("note_downloads");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NoteId).HasColumnName("note_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DownloadedAt).HasColumnName("downloaded_at");

            entity.HasOne(e => e.Note)
                .WithMany()
                .HasForeignKey(e => e.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.NoteId);
            entity.HasIndex(e => e.DownloadedAt);
        });
    }
}
