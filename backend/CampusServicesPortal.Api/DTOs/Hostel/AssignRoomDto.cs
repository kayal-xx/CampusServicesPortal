using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Hostel;

public class AssignRoomDto
{
    [Required]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "A valid room ID is required."
    )]
    public int RoomId { get; set; }
}