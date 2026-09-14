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

    // Admin - View all registration requests
    [HttpGet("admin")]
    public async Task<ActionResult<List<EventRegistrationDto>>>
        GetAllRegistrations()
    {
        List<EventRegistrationDto> registrations =
            await _eventService.GetAllRegistrationsAsync();

        return Ok(registrations);
    }

    // Admin - Approve registration
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> ApproveRegistration(int id)
    {
        var result =
            await _eventService.ApproveRegistrationAsync(id);

        if (!result.Success)
        {
            return NotFound(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    // Admin - Reject registration with reason
    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> RejectRegistration(
        int id,
        [FromBody] string rejectReason
    )
    {
        var result =
            await _eventService.RejectRegistrationAsync(
                id,
                rejectReason
            );

        if (!result.Success)
        {
            if (result.Message == "Registration not found.")
            {
                return NotFound(new
                {
                    message = result.Message
                });
            }

            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }
}