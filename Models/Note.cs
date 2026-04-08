namespace StudyNotesPlatform.Models;

public class Note
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int UniversityId { get; set; }
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public int StatusId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public int DownloadsCount { get; set; }
    public decimal? AverageRating { get; set; }

    // Навигация
    public User? User { get; set; }
    public University? University { get; set; }
    public Subject? Subject { get; set; }
    public Teacher? Teacher { get; set; }
    public NoteStatus? Status { get; set; }
}