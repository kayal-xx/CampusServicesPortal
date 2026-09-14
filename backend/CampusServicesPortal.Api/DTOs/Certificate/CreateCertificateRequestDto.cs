using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Certificate;

public class CreateCertificateRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Valid student ID is required.")]
    public int StudentId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one certificate is required.")]
    public List<CertificateDocumentRequestDto> Documents { get; set; } = new();
}

public class CertificateDocumentRequestDto
{
    [Required]
    public string CertificateType { get; set; } = string.Empty;

    [Required]
    [MinLength(5)]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Copies { get; set; } = 1;
}