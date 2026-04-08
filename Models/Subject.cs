namespace StudyNotesPlatform.Models;

public class Subject
{
    public int Id { get; set; }
    public int UniversityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public University? University { get; set; }
}