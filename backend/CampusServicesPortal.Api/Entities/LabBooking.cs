// LabBooking.cs
namespace CampusServicesPortal.Api.Entities;

public class LabBooking
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int LabId { get; set; }

    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public string Status { get; set; } = "Booked";

    public Student? Student { get; set; }
    public Lab? Lab { get; set; }
}