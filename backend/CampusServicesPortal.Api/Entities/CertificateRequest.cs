// CertificateRequest.cs
namespace CampusServicesPortal.Api.Entities;

public class CertificateRequest
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string CertificateType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;

    public int Copies { get; set; } = 1;

    public string Status { get; set; } = "Pending";

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
}