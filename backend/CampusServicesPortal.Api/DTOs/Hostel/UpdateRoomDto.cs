using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Hostel;

public class UpdateRoomDto
{
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