namespace StudyNotesPlatform.Models;

public class ModerationLog
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public int ModeratorId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public Note? Note { get; set; }
    public User? Moderator { get; set; }
}