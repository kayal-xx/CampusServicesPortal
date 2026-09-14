// Room.cs
namespace CampusServicesPortal.Api.Entities;

public class Room
{
    public int Id { get; set; }
    public int HostelId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public Hostel? Hostel { get; set; }
}