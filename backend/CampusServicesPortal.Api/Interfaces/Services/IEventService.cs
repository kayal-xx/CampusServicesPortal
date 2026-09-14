using CampusServicesPortal.Api.DTOs.Event;

namespace CampusServicesPortal.Api.Interfaces.Services;

public interface IEventService
{
    Task<List<EventDto>> GetAllAsync();

    Task<EventDto?> GetByIdAsync(int id);

    Task<EventDto> CreateAsync(CreateEventDto dto);

    Task<bool> UpdateAsync(int id, UpdateEventDto dto);

    Task<bool> DeleteAsync(int id);

    // Student
    Task<(bool Success, string Message, EventRegistrationDto? Data)>
        RegisterAsync(CreateEventRegistrationDto dto);

    Task<List<EventRegistrationDto>>
        GetStudentRegistrationsAsync(int studentId);

    Task<(bool Success, string Message)>
        CancelRegistrationAsync(int registrationId, int studentId);

    // Admin
    Task<List<EventRegistrationDto>>
        GetAllRegistrationsAsync();

    Task<(bool Success, string Message)>
        ApproveRegistrationAsync(int registrationId);

    Task<(bool Success, string Message)>
        RejectRegistrationAsync(
            int registrationId,
            string rejectReason
        );
}