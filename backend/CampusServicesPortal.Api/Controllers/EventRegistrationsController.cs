using CampusServicesPortal.Api.DTOs.Event;
using CampusServicesPortal.Api.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/event-registrations")]
public class EventRegistrationsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventRegistrationsController(
        IEventService eventService
    )
    {
        _eventService = eventService;
    }

    [HttpPost]
    public async Task<ActionResult<EventRegistrationDto>>
        Register(CreateEventRegistrationDto dto)
    {
        var result =
            await _eventService.RegisterAsync(dto);

        if (!result.Success)
        {
            if (result.Message == "Event not found.")
            {
                return NotFound(new
                {
                    message = result.Message
                });
            }

            return Conflict(new
            {
                message = result.Message
            });
        }

        return Created(
            $"/api/event-registrations/{result.Data!.Id}",
            result.Data
        );
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<List<EventRegistrationDto>>>
        GetStudentRegistrations(int studentId)
    {
        List<EventRegistrationDto> registrations =
            await _eventService
                .GetStudentRegistrationsAsync(studentId);

        return Ok(registrations);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(
        int id,
        [FromQuery] int studentId
    )
    {
        var result =
            await _eventService.CancelRegistrationAsync(
                id,
                studentId
            );

        if (!result.Success)
        {
            return NotFound(new
            {
                message = result.Message
            });
        }

        return NoContent();
    }
}