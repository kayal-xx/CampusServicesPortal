// Student.cs
namespace CampusServicesPortal.Api.Entities;

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string IndexNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}