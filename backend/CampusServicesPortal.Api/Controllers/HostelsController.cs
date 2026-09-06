using CampusServicesPortal.Api.DTOs.Hostel;
using CampusServicesPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/hostels")]
[Authorize]
public class HostelsController : ControllerBase
{
    private readonly HostelService _hostelService;

    public HostelsController(HostelService hostelService)
    {
        _hostelService = hostelService;
    }

    // Students and admins can view active hostels.
    [HttpGet]
    public async Task<ActionResult<List<HostelDto>>> GetHostels()
    {
        var hostels = await _hostelService.GetHostelsAsync();

        return Ok(hostels);
    }
    // Admin creates a new hostel.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HostelDto>> CreateHostel(
        CreateHostelDto request)
    {
        try
        {
            var hostel =
                await _hostelService.CreateHostelAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                hostel
            );
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }
    // Admin updates an existing hostel.
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HostelDto>> UpdateHostel(
        int id,
        UpdateHostelDto request)
    {
        try
        {
            var hostel =
                await _hostelService.UpdateHostelAsync(
                    id,
                    request
                );

            return Ok(hostel);
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

    // Admin deactivates a hostel.
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateHostel(int id)
    {
        try
        {
            await _hostelService.DeactivateHostelAsync(id);

            return Ok(new
            {
                message = "Hostel deactivated successfully."
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }
}