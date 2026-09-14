using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Hostel;

public class CreateRoomDto
{
    [Required]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "A valid hostel ID is required."
    )]
    public int HostelId { get; set; }

    [Required]
    [MaxLength(30)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [Range(
        1,
        20,
        ErrorMessage = "Room capacity must be between 1 and 20."
    )]
    public int Capacity { get; set; }
}