using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Hostel;

public class UpdateHostelApplicationStatusDto
{
    [Required]
    [RegularExpression(
        "^(Approved|Rejected)$",
        ErrorMessage = "Status must be Approved or Rejected."
    )]
    public string Status { get; set; } = string.Empty;
}