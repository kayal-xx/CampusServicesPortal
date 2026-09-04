using CampusServicesPortal.Api.DTOs.Event;

namespace CampusServicesPortal.Api.Interfaces.Services;

public interface IEventService
{
    Task<List<EventDto>> GetAllAsync();

    Task<EventDto?> GetByIdAsync(int id);

    Task<EventDto> CreateAsync(CreateEventDto dto);

    Task<bool> UpdateAsync(int id, UpdateEventDto dto);

    Task<bool> DeleteAsync(int id);

    Task<(bool Success, string Message, EventRegistrationDto? Data)>
        RegisterAsync(CreateEventRegistrationDto dto);

    Task<List<EventRegistrationDto>>
        GetStudentRegistrationsAsync(int studentId);

    Task<(bool Success, string Message)>
        CancelRegistrationAsync(int registrationId, int studentId);
}