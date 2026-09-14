namespace CampusServicesPortal.Api.DTOs.Hostel;

public class HostelDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}