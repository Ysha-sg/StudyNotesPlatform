using System;
using System.Data;

namespace StudyNotesPlatform.Models;

public class User
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int UniversityId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Role? Role { get; set; }
    public University? University { get; set; }
}