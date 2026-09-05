using CampusServicesPortal.Api.Entities;

namespace CampusServicesPortal.Api.Interfaces.Repositories;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(int id);
    Task<Event> CreateAsync(Event eventItem);
    Task<bool> UpdateAsync(Event eventItem);
    Task<bool> DeleteAsync(int id);

    Task<int> GetRegistrationCountAsync(int eventId);
    Task<bool> IsStudentRegisteredAsync(int studentId, int eventId);

    Task<EventRegistration> RegisterAsync(EventRegistration registration);

    Task<List<EventRegistration>> GetStudentRegistrationsAsync(
        int studentId
    );

    Task<EventRegistration?> GetRegistrationByIdAsync(int registrationId);

    Task<bool> CancelRegistrationAsync(
        int registrationId,
        int studentId
    );
}