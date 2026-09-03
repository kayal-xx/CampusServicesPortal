// EventRegistration.cs
namespace CampusServicesPortal.Api.Entities;

public class EventRegistration
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int EventId { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
    public Event? Event { get; set; }
}