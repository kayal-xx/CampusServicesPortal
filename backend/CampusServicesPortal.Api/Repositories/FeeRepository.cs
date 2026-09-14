using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Repositories;

public class FeeRepository : IFeeRepository
{
    private readonly ApplicationDbContext _context;

    public FeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FeePayment>> GetAllAsync()
    {
        return await _context.FeePayments
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FeePayment?> GetByIdAsync(int id)
    {
        return await _context.FeePayments
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<FeePayment>> GetByStudentIdAsync(int studentId)
    {
        return await _context.FeePayments
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<FeePayment> CreateAsync(FeePayment feePayment)
    {
        _context.FeePayments.Add(feePayment);
        await _context.SaveChangesAsync();

        return feePayment;
    }

    public async Task<FeePayment?> UpdateAsync(FeePayment feePayment)
    {
        _context.FeePayments.Update(feePayment);
        await _context.SaveChangesAsync();

        return feePayment;
    }

    public async Task<bool> ExistsAsync(int studentId, string feeType)
    {
        return await _context.FeePayments
            .AnyAsync(x =>
                x.StudentId == studentId &&
                x.FeeType == feeType);
    }
}