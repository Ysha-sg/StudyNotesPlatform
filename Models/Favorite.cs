using System.ComponentModel.DataAnnotations.Schema;

namespace StudyNotesPlatform.Models;

[Table("favorites")]
public class Favorite
{
    [Column("favorite_id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("note_id")]
    public int NoteId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Note? Note { get; set; }
}