using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Complaint;

public class UpdateComplaintStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? ResolutionNote { get; set; }
}