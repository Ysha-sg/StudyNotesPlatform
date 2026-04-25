namespace StudyNotesPlatform.Models;

public class NoteDownload
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public int UserId { get; set; }
    public DateTime DownloadedAt { get; set; }

    public Note? Note { get; set; }
    public User? User { get; set; }
}
