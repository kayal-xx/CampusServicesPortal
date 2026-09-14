// Hostel.cs
namespace CampusServicesPortal.Api.Entities;

public class Hostel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}