using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Complaint;

public class ComplaintDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ComplaintCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ResolutionNote { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ComplaintCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateComplaintCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateComplaintCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}