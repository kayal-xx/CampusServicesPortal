using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Repositories;

public class ComplaintRepository : IComplaintRepository
{
    private readonly ApplicationDbContext _context;

    public ComplaintRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComplaintCategory>>
        GetActiveCategoriesAsync()
    {
        return await _context.ComplaintCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<ComplaintCategory>>
        GetAllCategoriesAsync()
    {
        return await _context.ComplaintCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<ComplaintCategory?>
        GetCategoryByIdAsync(int id)
    {
        return await _context.ComplaintCategories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> CategoryNameExistsAsync(
        string name
    )
    {
        string normalizedName = name.Trim().ToLower();

        return await _context.ComplaintCategories.AnyAsync(
            c => c.Name.ToLower() == normalizedName
        );
    }

    public async Task<ComplaintCategory>
        CreateCategoryAsync(ComplaintCategory category)
    {
        _context.ComplaintCategories.Add(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<bool> UpdateCategoryAsync(
        ComplaintCategory category
    )
    {
        bool exists =
            await _context.ComplaintCategories.AnyAsync(
                c => c.Id == category.Id
            );

        if (!exists)
        {
            return false;
        }

        _context.ComplaintCategories.Update(category);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Complaint>>
        GetAllComplaintsAsync(string? status)
    {
        IQueryable<Complaint> query = _context.Complaints
            .AsNoTracking()
            .Include(c => c.ComplaintCategory);

        if (!string.IsNullOrWhiteSpace(status))
        {
            string normalizedStatus =
                status.Trim().ToLower();

            query = query.Where(
                c => c.Status.ToLower() == normalizedStatus
            );
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Complaint>>
        GetStudentComplaintsAsync(int studentId)
    {
        return await _context.Complaints
            .AsNoTracking()
            .Include(c => c.ComplaintCategory)
            .Where(c => c.StudentId == studentId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Complaint?>
        GetComplaintByIdAsync(int id)
    {
        return await _context.Complaints
            .Include(c => c.ComplaintCategory)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Complaint>
        CreateComplaintAsync(Complaint complaint)
    {
        _context.Complaints.Add(complaint);
        await _context.SaveChangesAsync();

        return complaint;
    }

    public async Task<bool> UpdateComplaintAsync(
        Complaint complaint
    )
    {
        bool exists = await _context.Complaints
            .AnyAsync(c => c.Id == complaint.Id);

        if (!exists)
        {
            return false;
        }

        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync();

        return true;
    }
}