using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Event;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public int AvailableSeats { get; set; }
    public bool IsFull { get; set; }
}

public class CreateEventDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Venue { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Range(1, 10000)]
    public int Capacity { get; set; }
}

public class UpdateEventDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Venue { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Range(1, 10000)]
    public int Capacity { get; set; }
}