using System.ComponentModel.DataAnnotations.Schema;

namespace StudyNotesPlatform.Models;

[Table("download_histories")]
public class DownloadHistory
{
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("note_id")]
    public int NoteId { get; set; }

    [Column("downloaded_at")]
    public DateTime DownloadedAt { get; set; }

    // Навигационные свойства
    public User? User { get; set; }
    public Note? Note { get; set; }
}