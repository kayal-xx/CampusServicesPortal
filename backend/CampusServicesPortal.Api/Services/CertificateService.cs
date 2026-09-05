using CampusServicesPortal.Api.DTOs.Certificate;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using CampusServicesPortal.Api.Interfaces.Services;

namespace CampusServicesPortal.Api.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certificateRepository;

    private static readonly string[] AllowedCertificateTypes =
    {
        "Bonafide Certificate",
        "Transcript",
        "Completion Letter"
    };

    private static readonly string[] AllowedStatuses =
    {
        "Pending",
        "Approved",
        "Rejected",
        "Ready for Collection"
    };

    public CertificateService(
        ICertificateRepository certificateRepository
    )
    {
        _certificateRepository = certificateRepository;
    }

    public async Task<List<CertificateRequestDto>>
        GetAllAsync(string? status)
    {
        List<CertificateRequest> requests =
            await _certificateRepository.GetAllAsync(status);

        return requests.Select(MapToDto).ToList();
    }

    public async Task<List<CertificateRequestDto>>
        GetByStudentAsync(int studentId)
    {
        List<CertificateRequest> requests =
            await _certificateRepository
                .GetByStudentAsync(studentId);

        return requests.Select(MapToDto).ToList();
    }

    public async Task<CertificateRequestDto?>
        GetByIdAsync(int id)
    {
        CertificateRequest? request =
            await _certificateRepository.GetByIdAsync(id);

        return request is null
            ? null
            : MapToDto(request);
    }

    public async Task<(
        bool Success,
        string Message,
        List<CertificateRequestDto>? Data
    )> CreateAsync(CreateCertificateRequestDto dto)
    {
        if (dto.Documents.Count == 0)
        {
            return (
                false,
                "At least one certificate is required.",
                null
            );
        }

        List<string> requestedTypes = dto.Documents
            .Select(document =>
                document.CertificateType.Trim())
            .ToList();

        bool containsDuplicateTypes =
            requestedTypes
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() != requestedTypes.Count;

        if (containsDuplicateTypes)
        {
            return (
                false,
                "The same certificate type cannot be requested twice in one submission.",
                null
            );
        }

        List<CertificateRequest> newRequests = new();

        foreach (CertificateDocumentRequestDto document
                 in dto.Documents)
        {
            string? validType =
                AllowedCertificateTypes.FirstOrDefault(
                    type => type.Equals(
                        document.CertificateType.Trim(),
                        StringComparison.OrdinalIgnoreCase
                    )
                );

            if (validType is null)
            {
                return (
                    false,
                    $"Invalid certificate type: {document.CertificateType}.",
                    null
                );
            }

            bool hasPendingRequest =
                await _certificateRepository
                    .HasPendingRequestAsync(
                        dto.StudentId,
                        validType
                    );

            if (hasPendingRequest)
            {
                return (
                    false,
                    $"A pending request already exists for {validType}.",
                    null
                );
            }

            newRequests.Add(new CertificateRequest
            {
                StudentId = dto.StudentId,
                CertificateType = validType,
                Reason = document.Reason.Trim(),
                Copies = document.Copies,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            });
        }

        List<CertificateRequest> createdRequests =
            await _certificateRepository
                .CreateManyAsync(newRequests);

        return (
            true,
            "Certificate request submitted successfully.",
            createdRequests.Select(MapToDto).ToList()
        );
    }

    public async Task<(bool Success, string Message)>
        UpdateStatusAsync(
            int id,
            UpdateCertificateRequestStatusDto dto
        )
    {
        CertificateRequest? request =
            await _certificateRepository.GetByIdAsync(id);

        if (request is null)
        {
            return (
                false,
                "Certificate request not found."
            );
        }

        string? validStatus =
            AllowedStatuses.FirstOrDefault(
                status => status.Equals(
                    dto.Status.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            );

        if (validStatus is null)
        {
            return (
                false,
                "Status must be Pending, Approved, Rejected, or Ready for Collection."
            );
        }

        if (validStatus == "Ready for Collection" &&
            request.Status != "Approved")
        {
            return (
                false,
                "Only an approved certificate can be marked as ready for collection."
            );
        }

        request.Status = validStatus;

        await _certificateRepository.UpdateAsync(request);

        return (
            true,
            "Certificate request status updated successfully."
        );
    }

    private static CertificateRequestDto MapToDto(
        CertificateRequest request
    )
    {
        return new CertificateRequestDto
        {
            Id = request.Id,
            StudentId = request.StudentId,
            CertificateType = request.CertificateType,
            Reason = request.Reason,
            Copies = request.Copies,
            Status = request.Status,
            RequestedAt = request.RequestedAt
        };
    }
}