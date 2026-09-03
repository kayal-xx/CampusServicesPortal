// HostelApplication.cs
namespace CampusServicesPortal.Api.Entities;

public class HostelApplication
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int HostelId { get; set; }
    public int? RoomId { get; set; }

    public string Semester { get; set; } = string.Empty;
    public string SpecialRequirements { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
    public Hostel? Hostel { get; set; }
    public Room? Room { get; set; }
}