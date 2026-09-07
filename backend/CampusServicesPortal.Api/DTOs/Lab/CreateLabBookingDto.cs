using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Lab;

public class CreateLabBookingDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int LabId { get; set; }

    [Required]
    public DateTime BookingDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }
}