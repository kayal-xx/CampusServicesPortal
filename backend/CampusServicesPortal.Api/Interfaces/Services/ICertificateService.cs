using CampusServicesPortal.Api.DTOs.Certificate;

namespace CampusServicesPortal.Api.Interfaces.Services;

public interface ICertificateService
{
    Task<List<CertificateRequestDto>>
        GetAllAsync(string? status);

    Task<List<CertificateRequestDto>>
        GetByStudentAsync(int studentId);

    Task<CertificateRequestDto?> GetByIdAsync(int id);

    Task<(
        bool Success,
        string Message,
        List<CertificateRequestDto>? Data
    )> CreateAsync(CreateCertificateRequestDto dto);

    Task<(bool Success, string Message)>
        UpdateStatusAsync(
            int id,
            UpdateCertificateRequestStatusDto dto
        );
}