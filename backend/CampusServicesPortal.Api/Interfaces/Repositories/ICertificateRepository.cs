using CampusServicesPortal.Api.Entities;

namespace CampusServicesPortal.Api.Interfaces.Repositories;

public interface ICertificateRepository
{
    Task<List<CertificateRequest>> GetAllAsync(
        string? status
    );

    Task<List<CertificateRequest>> GetByStudentAsync(
        int studentId
    );

    Task<CertificateRequest?> GetByIdAsync(int id);

    Task<bool> HasPendingRequestAsync(
        int studentId,
        string certificateType
    );

    Task<List<CertificateRequest>> CreateManyAsync(
        List<CertificateRequest> requests
    );

    Task<bool> UpdateAsync(
        CertificateRequest request
    );
}