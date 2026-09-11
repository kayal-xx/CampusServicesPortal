using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Lab;

public class UpdateLabBookingStatusDto
{
    [Required]
    [RegularExpression(
        "^(Approved|Rejected)$",
        ErrorMessage = "Status must be Approved or Rejected."
    )]
    public string Status { get; set; } = string.Empty;

    public string RejectionReason { get; set; } = string.Empty;
}