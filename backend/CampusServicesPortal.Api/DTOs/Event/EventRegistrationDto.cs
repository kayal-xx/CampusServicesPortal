using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Event;

public class EventRegistrationDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public DateTime RegisteredAt { get; set; }
}

public class CreateEventRegistrationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Valid student ID is required.")]
    public int StudentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Valid event ID is required.")]
    public int EventId { get; set; }
}