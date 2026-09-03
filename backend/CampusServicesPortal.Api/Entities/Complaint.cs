// Complaint.cs
namespace CampusServicesPortal.Api.Entities;

public class Complaint
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public int ComplaintCategoryId { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? ResolutionNote { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
    public ComplaintCategory? ComplaintCategory { get; set; }
}