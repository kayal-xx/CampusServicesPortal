using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Repositories;

public class HostelRepository
{
    private readonly ApplicationDbContext _context;

    public HostelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hostel>> GetActiveHostelsAsync()
    {
        return await _context.Hostels
            .Where(hostel => hostel.IsActive)
            .OrderBy(hostel => hostel.Name)
            .ToListAsync();
    }

    public async Task<Hostel?> GetHostelByIdAsync(int hostelId)
    {
        return await _context.Hostels
            .FirstOrDefaultAsync(hostel =>
                hostel.Id == hostelId &&
                hostel.IsActive);
    }

    public async Task<List<Room>> GetRoomsByHostelAsync(
        int hostelId)
    {
        return await _context.Rooms
            .Include(room => room.Hostel)
            .Where(room => room.HostelId == hostelId)
            .OrderBy(room => room.RoomNumber)
            .ToListAsync();
    }

    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        return await _context.Rooms
            .Include(room => room.Hostel)
            .FirstOrDefaultAsync(room => room.Id == roomId);
    }

    public async Task<int> GetRoomOccupancyAsync(int roomId)
    {
        return await _context.HostelApplications
            .CountAsync(application =>
                application.RoomId == roomId &&
                application.Status == "Room Assigned");
    }

    public async Task<HostelApplication?> GetApplicationByIdAsync(
        int applicationId)
    {
        return await _context.HostelApplications
            .Include(application => application.Student)
            .Include(application => application.Hostel)
            .Include(application => application.Room)
            .FirstOrDefaultAsync(application =>
                application.Id == applicationId);
    }

    public async Task<List<HostelApplication>>
        GetApplicationsByStudentAsync(int studentId)
    {
        return await _context.HostelApplications
            .Include(application => application.Student)
            .Include(application => application.Hostel)
            .Include(application => application.Room)
            .Where(application =>
                application.StudentId == studentId)
            .OrderByDescending(application =>
                application.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<HostelApplication>>
        GetApplicationsAsync(string? status)
    {
        IQueryable<HostelApplication> query =
            _context.HostelApplications
                .Include(application => application.Student)
                .Include(application => application.Hostel)
                .Include(application => application.Room);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim();

            query = query.Where(application =>
                application.Status == normalizedStatus);
        }

        return await query
            .OrderByDescending(application =>
                application.CreatedAt)
            .ToListAsync();
    }

    public async Task AddApplicationAsync(
        HostelApplication application)
    {
        await _context.HostelApplications.AddAsync(application);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateApplicationAsync(
        HostelApplication application)
    {
        _context.HostelApplications.Update(application);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> HostelNameExistsAsync(string name)
    {
        var normalizedName = name.Trim().ToLower();

        return await _context.Hostels
            .AnyAsync(hostel =>
                hostel.Name.ToLower() == normalizedName);
    }

    public async Task AddHostelAsync(Hostel hostel)
    {
        await _context.Hostels.AddAsync(hostel);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RoomNumberExistsAsync(
        int hostelId,
        string roomNumber)
    {
        var normalizedRoomNumber =
            roomNumber.Trim().ToLower();

        return await _context.Rooms
            .AnyAsync(room =>
                room.HostelId == hostelId &&
                room.RoomNumber.ToLower()
                    == normalizedRoomNumber);
    }

    public async Task AddRoomAsync(Room room)
    {
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> HostelNameExistsForOtherHostelAsync(
        string name,
        int hostelId)
    {
        var normalizedName = name.Trim().ToLower();

        return await _context.Hostels
            .AnyAsync(hostel =>
                hostel.Id != hostelId &&
                hostel.Name.ToLower() == normalizedName);
    }

    public async Task UpdateHostelAsync(Hostel hostel)
    {
        _context.Hostels.Update(hostel);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RoomNumberExistsForOtherRoomAsync(
        int hostelId,
        string roomNumber,
        int roomId)
    {
        var normalizedRoomNumber =
            roomNumber.Trim().ToLower();

        return await _context.Rooms
            .AnyAsync(room =>
                room.Id != roomId &&
                room.HostelId == hostelId &&
                room.RoomNumber.ToLower()
                    == normalizedRoomNumber);
    }

    public async Task UpdateRoomAsync(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }
}