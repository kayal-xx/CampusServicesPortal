using CampusServicesPortal.Api.Data;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Api.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllAsync()
    {
        return await _context.Events
            .AsNoTracking()
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event> CreateAsync(Event eventItem)
    {
        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        return eventItem;
    }

    public async Task<bool> UpdateAsync(Event eventItem)
    {
        bool exists = await _context.Events
            .AnyAsync(e => e.Id == eventItem.Id);

        if (!exists)
        {
            return false;
        }

        _context.Events.Update(eventItem);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Event? eventItem = await _context.Events.FindAsync(id);

        if (eventItem is null)
        {
            return false;
        }

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetRegistrationCountAsync(int eventId)
    {
        return await _context.EventRegistrations
            .CountAsync(r => r.EventId == eventId);
    }

    public async Task<bool> IsStudentRegisteredAsync(
        int studentId,
        int eventId
    )
    {
        return await _context.EventRegistrations.AnyAsync(
            r => r.StudentId == studentId &&
                 r.EventId == eventId
        );
    }

    public async Task<EventRegistration> RegisterAsync(
        EventRegistration registration
    )
    {
        _context.EventRegistrations.Add(registration);
        await _context.SaveChangesAsync();

        return registration;
    }

    public async Task<List<EventRegistration>>
        GetStudentRegistrationsAsync(int studentId)
    {
        return await _context.EventRegistrations
            .AsNoTracking()
            .Include(r => r.Event)
            .Where(r => r.StudentId == studentId)
            .OrderBy(r => r.Event!.EventDate)
            .ToListAsync();
    }

    public async Task<EventRegistration?>
        GetRegistrationByIdAsync(int registrationId)
    {
        return await _context.EventRegistrations
            .AsNoTracking()
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.Id == registrationId);
    }

    public async Task<bool> CancelRegistrationAsync(
        int registrationId,
        int studentId
    )
    {
        EventRegistration? registration =
            await _context.EventRegistrations.FirstOrDefaultAsync(
                r => r.Id == registrationId &&
                     r.StudentId == studentId
            );

        if (registration is null)
        {
            return false;
        }

        _context.EventRegistrations.Remove(registration);
        await _context.SaveChangesAsync();

        return true;
    }
}