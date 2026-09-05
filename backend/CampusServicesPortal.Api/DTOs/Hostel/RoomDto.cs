namespace CampusServicesPortal.Api.DTOs.Hostel;

public class RoomDto
{
    public int Id { get; set; }

    public int HostelId { get; set; }

    public string HostelName { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int CurrentOccupancy { get; set; }

    public int AvailableSpaces =>
        Math.Max(0, Capacity - CurrentOccupancy);
}