using CampusServicesPortal.Api.DTOs.Hostel;
using CampusServicesPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/hostels/{hostelId:int}/rooms")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly HostelService _hostelService;

    public RoomsController(HostelService hostelService)
    {
        _hostelService = hostelService;
    }

    // Students and admins can view rooms in a hostel.
    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetRooms(
        int hostelId)
    {
        try
        {
            var rooms =
                await _hostelService.GetRoomsByHostelAsync(
                    hostelId
                );

            return Ok(rooms);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }
    // Admin creates a new room.
    [HttpPost("~/api/rooms")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> CreateRoom(
        CreateRoomDto request)
    {
        try
        {
            var room =
                await _hostelService.CreateRoomAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                room
            );
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
    // Admin updates an existing room.
    [HttpPut("~/api/rooms/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> UpdateRoom(
        int id,
        UpdateRoomDto request)
    {
        try
        {
            var room =
                await _hostelService.UpdateRoomAsync(
                    id,
                    request
                );

            return Ok(room);
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
}