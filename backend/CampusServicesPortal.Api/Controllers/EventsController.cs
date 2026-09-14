using CampusServicesPortal.Api.DTOs.Event;
using CampusServicesPortal.Api.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventDto>>> GetAll(
        [FromQuery] string? period
    )
    {
        List<EventDto> events = await _eventService.GetAllAsync();

        if (period?.ToLower() == "upcoming")
        {
            events = events
                .Where(e => e.EventDate >= DateTime.UtcNow)
                .ToList();
        }
        else if (period?.ToLower() == "past")
        {
            events = events
                .Where(e => e.EventDate < DateTime.UtcNow)
                .ToList();
        }

        return Ok(events);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventDto>> GetById(int id)
    {
        EventDto? eventItem =
            await _eventService.GetByIdAsync(id);

        if (eventItem is null)
        {
            return NotFound(new
            {
                message = "Event not found."
            });
        }

        return Ok(eventItem);
    }

    [HttpPost]
    public async Task<ActionResult<EventDto>> Create(
        CreateEventDto dto
    )
    {
        if (dto.EventDate <= DateTime.UtcNow)
        {
            return BadRequest(new
            {
                message = "Event date must be in the future."
            });
        }

        EventDto createdEvent =
            await _eventService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdEvent.Id },
            createdEvent
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateEventDto dto
    )
    {
        if (dto.EventDate <= DateTime.UtcNow)
        {
            return BadRequest(new
            {
                message = "Event date must be in the future."
            });
        }

        try
        {
            bool updated =
                await _eventService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _eventService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Event not found."
            });
        }

        return NoContent();
    }
}