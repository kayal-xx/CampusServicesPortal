namespace CampusServicesPortal.Api.DTOs.Lab;

public class LabBookingDto
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int LabId { get; set; }

    public string LabName { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string Status { get; set; } = string.Empty;
}