using CampusServicesPortal.Api.DTOs.Fee;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using CampusServicesPortal.Api.Interfaces.Services;

namespace CampusServicesPortal.Api.Services;

public class FeeService : IFeeService
{
    private readonly IFeeRepository _feeRepository;

    public FeeService(IFeeRepository feeRepository)
    {
        _feeRepository = feeRepository;
    }

    public async Task<List<FeePaymentDto>> GetAllAsync()
    {
        var fees = await _feeRepository.GetAllAsync();

        return fees.Select(MapToDto).ToList();
    }

    public async Task<FeePaymentDto?> GetByIdAsync(int id)
    {
        var fee = await _feeRepository.GetByIdAsync(id);

        return fee == null ? null : MapToDto(fee);
    }

    public async Task<List<FeePaymentDto>> GetByStudentIdAsync(int studentId)
    {
        var fees = await _feeRepository.GetByStudentIdAsync(studentId);

        return fees.Select(MapToDto).ToList();
    }

    public async Task<FeePaymentDto> CreateAsync(CreateFeePaymentDto dto)
    {
        var exists = await _feeRepository.ExistsAsync(
            dto.StudentId,
            dto.FeeType
        );

        if (exists)
        {
            throw new InvalidOperationException(
                "A fee record already exists for this student and fee type."
            );
        }

        var fee = new FeePayment
        {
            StudentId = dto.StudentId,
            FeeType = dto.FeeType,
            Amount = dto.Amount,
            IsPaid = false,
            PaidAt = null
        };

        var createdFee = await _feeRepository.CreateAsync(fee);

        return MapToDto(createdFee);
    }

    public async Task<FeePaymentDto?> UpdateStatusAsync(
        int id,
        UpdateFeePaymentStatusDto dto)
    {
        var fee = await _feeRepository.GetByIdAsync(id);

        if (fee == null)
        {
            return null;
        }

        fee.IsPaid = dto.IsPaid;

        fee.PaidAt = dto.IsPaid
            ? DateTime.UtcNow
            : null;

        var updatedFee = await _feeRepository.UpdateAsync(fee);

        return updatedFee == null
            ? null
            : MapToDto(updatedFee);
    }

    private static FeePaymentDto MapToDto(FeePayment fee)
    {
        return new FeePaymentDto
        {
            Id = fee.Id,
            StudentId = fee.StudentId,
            FeeType = fee.FeeType,
            Amount = fee.Amount,
            IsPaid = fee.IsPaid,
            PaidAt = fee.PaidAt
        };
    }
}