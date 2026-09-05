using CampusServicesPortal.Api.DTOs.Event;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using CampusServicesPortal.Api.Interfaces.Services;

namespace CampusServicesPortal.Api.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<List<EventDto>> GetAllAsync()
    {
        List<Event> events = await _eventRepository.GetAllAsync();
        List<EventDto> result = new();

        foreach (Event eventItem in events)
        {
            result.Add(await MapToEventDtoAsync(eventItem));
        }

        return result;
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        Event? eventItem = await _eventRepository.GetByIdAsync(id);

        if (eventItem is null)
        {
            return null;
        }

        return await MapToEventDtoAsync(eventItem);
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        Event eventItem = new()
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Venue = dto.Venue.Trim(),
            EventDate = dto.EventDate,
            Capacity = dto.Capacity
        };

        Event createdEvent =
            await _eventRepository.CreateAsync(eventItem);

        return await MapToEventDtoAsync(createdEvent);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateEventDto dto
    )
    {
        Event? eventItem = await _eventRepository.GetByIdAsync(id);

        if (eventItem is null)
        {
            return false;
        }

        int registeredCount =
            await _eventRepository.GetRegistrationCountAsync(id);

        if (dto.Capacity < registeredCount)
        {
            throw new InvalidOperationException(
                "Capacity cannot be lower than the current registration count."
            );
        }

        eventItem.Title = dto.Title.Trim();
        eventItem.Description = dto.Description.Trim();
        eventItem.Venue = dto.Venue.Trim();
        eventItem.EventDate = dto.EventDate;
        eventItem.Capacity = dto.Capacity;

        return await _eventRepository.UpdateAsync(eventItem);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _eventRepository.DeleteAsync(id);
    }

    public async Task<(
        bool Success,
        string Message,
        EventRegistrationDto? Data
    )> RegisterAsync(CreateEventRegistrationDto dto)
    {
        Event? eventItem =
            await _eventRepository.GetByIdAsync(dto.EventId);

        if (eventItem is null)
        {
            return (false, "Event not found.", null);
        }

        if (eventItem.EventDate <= DateTime.UtcNow)
        {
            return (
                false,
                "Registration is closed for this event.",
                null
            );
        }

        bool alreadyRegistered =
            await _eventRepository.IsStudentRegisteredAsync(
                dto.StudentId,
                dto.EventId
            );

        if (alreadyRegistered)
        {
            return (
                false,
                "Student is already registered for this event.",
                null
            );
        }

        int registeredCount =
            await _eventRepository.GetRegistrationCountAsync(
                dto.EventId
            );

        if (registeredCount >= eventItem.Capacity)
        {
            return (
                false,
                "Event capacity has been reached.",
                null
            );
        }

        EventRegistration registration = new()
        {
            StudentId = dto.StudentId,
            EventId = dto.EventId,
            RegisteredAt = DateTime.UtcNow
        };

        EventRegistration createdRegistration =
            await _eventRepository.RegisterAsync(registration);

        EventRegistrationDto result = new()
        {
            Id = createdRegistration.Id,
            StudentId = createdRegistration.StudentId,
            EventId = createdRegistration.EventId,
            EventTitle = eventItem.Title,
            EventDate = eventItem.EventDate,
            Venue = eventItem.Venue,
            RegisteredAt = createdRegistration.RegisteredAt
        };

        return (
            true,
            "Event registration completed successfully.",
            result
        );
    }

    public async Task<List<EventRegistrationDto>>
        GetStudentRegistrationsAsync(int studentId)
    {
        List<EventRegistration> registrations =
            await _eventRepository
                .GetStudentRegistrationsAsync(studentId);

        return registrations.Select(r =>
            new EventRegistrationDto
            {
                Id = r.Id,
                StudentId = r.StudentId,
                EventId = r.EventId,
                EventTitle = r.Event?.Title ?? string.Empty,
                EventDate = r.Event?.EventDate ?? default,
                Venue = r.Event?.Venue ?? string.Empty,
                RegisteredAt = r.RegisteredAt
            }
        ).ToList();
    }

    public async Task<(bool Success, string Message)>
        CancelRegistrationAsync(
            int registrationId,
            int studentId
        )
    {
        bool cancelled =
            await _eventRepository.CancelRegistrationAsync(
                registrationId,
                studentId
            );

        if (!cancelled)
        {
            return (
                false,
                "Registration not found or it does not belong to this student."
            );
        }

        return (
            true,
            "Event registration cancelled successfully."
        );
    }

    private async Task<EventDto> MapToEventDtoAsync(
        Event eventItem
    )
    {
        int registeredCount =
            await _eventRepository.GetRegistrationCountAsync(
                eventItem.Id
            );

        int availableSeats =
            Math.Max(eventItem.Capacity - registeredCount, 0);

        return new EventDto
        {
            Id = eventItem.Id,
            Title = eventItem.Title,
            Description = eventItem.Description,
            Venue = eventItem.Venue,
            EventDate = eventItem.EventDate,
            Capacity = eventItem.Capacity,
            RegisteredCount = registeredCount,
            AvailableSeats = availableSeats,
            IsFull = registeredCount >= eventItem.Capacity
        };
    }
}