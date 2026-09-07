using System.Security.Claims;
using CampusServicesPortal.Api.DTOs.Hostel;
using CampusServicesPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/hostel-applications")]
[Authorize]
public class HostelApplicationsController : ControllerBase
{
    private readonly HostelService _hostelService;

    public HostelApplicationsController(
        HostelService hostelService)
    {
        _hostelService = hostelService;
    }

    // Logged-in student submits an application.
    [HttpPost]
    public async Task<ActionResult<HostelApplicationDto>>
        CreateApplication(CreateHostelApplicationDto request)
    {
        var studentId = GetLoggedInStudentId();

        if (studentId is null)
        {
            return Unauthorized(new
            {
                message = "Valid student identity was not found."
            });
        }

        try
        {
            var application =
                await _hostelService.CreateApplicationAsync(
                    studentId.Value,
                    request
                );

            return StatusCode(
                StatusCodes.Status201Created,
                application
            );
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    // Student can view own applications.
    // Admin can view any student's applications.
    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<List<HostelApplicationDto>>>
        GetStudentApplications(int studentId)
    {
        if (!IsAdmin() &&
            GetLoggedInStudentId() != studentId)
        {
            return Forbid();
        }

        var applications =
            await _hostelService.GetStudentApplicationsAsync(
                studentId
            );

        return Ok(applications);
    }

    // Admin can view all applications and filter by status.
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<HostelApplicationDto>>>
        GetAllApplications([FromQuery] string? status)
    {
        var applications =
            await _hostelService.GetAllApplicationsAsync(status);

        return Ok(applications);
    }

    // Admin approves or rejects an application.
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HostelApplicationDto>>
        UpdateStatus(
            int id,
            UpdateHostelApplicationStatusDto request)
    {
        try
        {
            var application =
                await _hostelService.UpdateStatusAsync(
                    id,
                    request
                );

            return Ok(application);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    // Admin assigns a room after approval.
    [HttpPut("{id:int}/assign-room")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HostelApplicationDto>>
        AssignRoom(int id, AssignRoomDto request)
    {
        try
        {
            var application =
                await _hostelService.AssignRoomAsync(
                    id,
                    request
                );

            return Ok(application);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    private int? GetLoggedInStudentId()
    {
        var claimValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        return int.TryParse(claimValue, out var studentId)
            ? studentId
            : null;
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }
}