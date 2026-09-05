using CampusServicesPortal.Api.DTOs.Fee;

namespace CampusServicesPortal.Api.Interfaces.Services;

public interface IFeeService
{
    Task<List<FeePaymentDto>> GetAllAsync();

    Task<FeePaymentDto?> GetByIdAsync(int id);

    Task<List<FeePaymentDto>> GetByStudentIdAsync(int studentId);

    Task<FeePaymentDto> CreateAsync(CreateFeePaymentDto dto);

    Task<FeePaymentDto?> UpdateStatusAsync(
        int id,
        UpdateFeePaymentStatusDto dto
    );
}