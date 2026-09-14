using CampusServicesPortal.Api.Entities;

namespace CampusServicesPortal.Api.Interfaces.Repositories;

public interface IFeeRepository
{
    Task<List<FeePayment>> GetAllAsync();

    Task<FeePayment?> GetByIdAsync(int id);

    Task<List<FeePayment>> GetByStudentIdAsync(int studentId);

    Task<FeePayment> CreateAsync(FeePayment feePayment);

    Task<FeePayment?> UpdateAsync(FeePayment feePayment);

    Task<bool> ExistsAsync(int studentId, string feeType);
}