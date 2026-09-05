namespace CampusServicesPortal.Api.DTOs.Certificate;

public class CertificateRequestDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string CertificateType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int Copies { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
}