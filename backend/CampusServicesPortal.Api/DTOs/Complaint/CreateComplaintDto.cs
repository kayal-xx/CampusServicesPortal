using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Complaint;

public class CreateComplaintDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Valid student ID is required.")]
    public int StudentId { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Valid complaint category ID is required."
    )]
    public int ComplaintCategoryId { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
}