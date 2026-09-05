using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Repositories;

public class StudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .FirstOrDefaultAsync(student => student.Id == id);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _context.Students
            .FirstOrDefaultAsync(student =>
                student.Email.ToLower() == normalizedEmail);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _context.Students
            .AnyAsync(student =>
                student.Email.ToLower() == normalizedEmail);
    }

    public async Task<bool> IndexNumberExistsAsync(
        string indexNumber)
    {
        var normalizedIndexNumber =
            indexNumber.Trim().ToUpper();

        return await _context.Students
            .AnyAsync(student =>
                student.IndexNumber.ToUpper()
                    == normalizedIndexNumber);
    }

    public async Task<bool> EmailExistsForOtherStudentAsync(
        string email,
        int studentId)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _context.Students
            .AnyAsync(student =>
                student.Id != studentId &&
                student.Email.ToLower() == normalizedEmail);
    }

    public async Task<List<Student>> SearchAsync(
        string? search,
        string? faculty)
    {
        IQueryable<Student> query = _context.Students;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchValue = search.Trim();

            query = query.Where(student =>
                student.FullName.Contains(searchValue) ||
                student.IndexNumber.Contains(searchValue));
        }

        if (!string.IsNullOrWhiteSpace(faculty))
        {
            var facultyValue = faculty.Trim();

            query = query.Where(student =>
                student.Faculty == facultyValue);
        }

        return await query
            .OrderBy(student => student.FullName)
            .ToListAsync();
    }

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(student => student.Id == id);

        if (student is null)
        {
            return false;
        }

        student.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}