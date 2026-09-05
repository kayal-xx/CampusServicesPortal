using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Certificate;

public class UpdateCertificateRequestStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}