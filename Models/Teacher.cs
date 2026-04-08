namespace StudyNotesPlatform.Models;

public class Teacher
{
    public int Id { get; set; }
    public int UniversityId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public University? University { get; set; }
}