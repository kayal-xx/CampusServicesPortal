using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Repositories;

public class LabRepository
{
    private readonly ApplicationDbContext _context;

    public LabRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lab>> GetActiveLabsAsync()
    {
        return await _context.Labs
            .Where(lab => lab.IsActive)
            .OrderBy(lab => lab.Name)
            .ToListAsync();
    }

    public async Task<Lab?> GetLabByIdAsync(int labId)
    {
        return await _context.Labs
            .FirstOrDefaultAsync(lab =>
                lab.Id == labId &&
                lab.IsActive);
    }

    public async Task<LabBooking?> GetBookingByIdAsync(int bookingId)
    {
        return await _context.LabBookings
            .Include(booking => booking.Student)
            .Include(booking => booking.Lab)
            .FirstOrDefaultAsync(booking =>
                booking.Id == bookingId);
    }

    public async Task<List<LabBooking>> GetBookingsByStudentAsync(
        int studentId)
    {
        return await _context.LabBookings
            .Include(booking => booking.Lab)
            .Where(booking =>
                booking.StudentId == studentId)
            .OrderByDescending(booking =>
                booking.BookingDate)
            .ThenBy(booking =>
                booking.StartTime)
            .ToListAsync();
    }
    public async Task<List<LabBooking>> GetAllBookingsAsync()
    {
        return await _context.LabBookings
            .Include(booking => booking.Student)
            .Include(booking => booking.Lab)
            .OrderByDescending(booking => booking.BookingDate)
            .ThenBy(booking => booking.StartTime)
            .ToListAsync();
    }
    public async Task<bool> HasConflictingBookingAsync(
        int labId,
        DateTime bookingDate,
        TimeSpan startTime,
        TimeSpan endTime)
    {
        var date = bookingDate.Date;

        return await _context.LabBookings
            .AnyAsync(booking =>
                booking.LabId == labId &&
                booking.BookingDate.Date == date &&
                booking.Status != "Cancelled" &&
                startTime < booking.EndTime &&
                endTime > booking.StartTime);
    }

    public async Task AddBookingAsync(LabBooking booking)
    {
        await _context.LabBookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBookingAsync(LabBooking booking)
    {
        _context.LabBookings.Update(booking);
        await _context.SaveChangesAsync();
    }
    public async Task<Lab?> GetLabForAdminAsync(int labId)
    {
        return await _context.Labs
            .FirstOrDefaultAsync(lab => lab.Id == labId);
    }

    public async Task<bool> LabNameExistsAsync(
        string name,
        int? excludeId = null)
    {
        return await _context.Labs.AnyAsync(lab =>
            lab.Name.ToLower() == name.ToLower() &&
            (!excludeId.HasValue || lab.Id != excludeId.Value));
    }

    public async Task AddLabAsync(Lab lab)
    {
        await _context.Labs.AddAsync(lab);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLabAsync(Lab lab)
    {
        _context.Labs.Update(lab);
        await _context.SaveChangesAsync();
    }
}