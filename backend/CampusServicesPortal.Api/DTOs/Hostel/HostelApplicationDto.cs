namespace CampusServicesPortal.Api.DTOs.Hostel;

public class HostelApplicationDto
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int HostelId { get; set; }

    public string HostelName { get; set; } = string.Empty;

    public int? RoomId { get; set; }

    public string? RoomNumber { get; set; }

    public string Semester { get; set; } = string.Empty;

    public string SpecialRequirements { get; set; }
        = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}