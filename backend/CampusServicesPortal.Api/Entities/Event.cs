// Event.cs
namespace CampusServicesPortal.Api.Entities;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    public int Capacity { get; set; }
}