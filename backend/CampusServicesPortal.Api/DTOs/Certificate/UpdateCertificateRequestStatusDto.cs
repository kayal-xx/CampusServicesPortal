using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Certificate;

public class UpdateCertificateRequestStatusDto
{
    [Required]
    [RegularExpression(
        "^(Approved|Rejected|Pending)$",
        ErrorMessage = "Status must be Approved, Rejected, or Pending."
    )]
    public string Status { get; set; } = string.Empty;

    public string RejectionReason { get; set; } = string.Empty;
}