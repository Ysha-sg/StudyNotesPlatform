namespace StudyNotesPlatform.Models;

public class Complaint
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public int ReporterId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string Status { get; set; } = "open";
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int? ResolvedById { get; set; }
    public string? ResolutionComment { get; set; }

    public Note? Note { get; set; }
    public User? Reporter { get; set; }
    public User? ResolvedBy { get; set; }
}

