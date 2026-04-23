namespace StudyNotesPlatform.Models;

public class NoteRating
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Note? Note { get; set; }
    public User? User { get; set; }
}
