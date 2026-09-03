// Notification.cs
namespace CampusServicesPortal.Api.Entities;

public class Notification
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
}