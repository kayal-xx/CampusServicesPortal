using System.Security.Claims;
using CampusServicesPortal.Api.DTOs.Lab;
using CampusServicesPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/lab-bookings")]
[Authorize]
public class LabBookingsController : ControllerBase
{
    private readonly LabService _labService;

    public LabBookingsController(LabService labService)
    {
        _labService = labService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        CreateLabBookingDto dto)
    {
        try
        {
            var studentId = GetStudentId();

            var booking =
                await _labService.CreateBookingAsync(studentId, dto);

            return StatusCode(StatusCodes.Status201Created, booking);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var studentId = GetStudentId();

        var bookings =
            await _labService.GetStudentBookingsAsync(studentId);

        return Ok(bookings);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _labService.GetAllBookingsAsync();

        return Ok(bookings);
    }
    [HttpPut("{bookingId:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        try
        {
            var studentId = GetStudentId();

            await _labService.CancelBookingAsync(
                bookingId,
                studentId);

            return Ok(new
            {
                message = "Lab booking cancelled successfully."
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (UnauthorizedAccessException exception)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
    // Admin approves or rejects a lab booking.
    [HttpPut("{bookingId:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(
        int bookingId,
        UpdateLabBookingStatusDto request)
    {
        try
        {
            var booking =
                await _labService.UpdateStatusAsync(
                    bookingId,
                    request);

            return Ok(booking);
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
    private int GetStudentId()
    {
        var studentIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(studentIdValue, out var studentId))
        {
            throw new UnauthorizedAccessException(
                "Invalid authentication token.");
        }

        return studentId;
    }
}