using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Repositories;

public class CertificateRepository : ICertificateRepository
{
    private readonly ApplicationDbContext _context;

    public CertificateRepository(
        ApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<List<CertificateRequest>> GetAllAsync(
        string? status
    )
    {
        IQueryable<CertificateRequest> query =
            _context.CertificateRequests
                .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            string normalizedStatus =
                status.Trim().ToLower();

            query = query.Where(
                request =>
                    request.Status.ToLower()
                    == normalizedStatus
            );
        }

        return await query
            .OrderByDescending(request =>
                request.RequestedAt)
            .ToListAsync();
    }

    public async Task<List<CertificateRequest>>
        GetByStudentAsync(int studentId)
    {
        return await _context.CertificateRequests
            .AsNoTracking()
            .Where(request =>
                request.StudentId == studentId)
            .OrderByDescending(request =>
                request.RequestedAt)
            .ToListAsync();
    }

    public async Task<CertificateRequest?>
        GetByIdAsync(int id)
    {
        return await _context.CertificateRequests
            .FirstOrDefaultAsync(request =>
                request.Id == id);
    }

    public async Task<bool> HasPendingRequestAsync(
        int studentId,
        string certificateType
    )
    {
        string normalizedType =
            certificateType.Trim().ToLower();

        return await _context.CertificateRequests
            .AnyAsync(request =>
                request.StudentId == studentId &&
                request.CertificateType.ToLower()
                    == normalizedType &&
                request.Status == "Pending"
            );
    }

    public async Task<List<CertificateRequest>>
        CreateManyAsync(
            List<CertificateRequest> requests
        )
    {
        _context.CertificateRequests.AddRange(requests);
        await _context.SaveChangesAsync();

        return requests;
    }

    public async Task<bool> UpdateAsync(
        CertificateRequest request
    )
    {
        bool exists =
            await _context.CertificateRequests
                .AnyAsync(item =>
                    item.Id == request.Id);

        if (!exists)
        {
            return false;
        }

        _context.CertificateRequests.Update(request);
        await _context.SaveChangesAsync();

        return true;
    }
}