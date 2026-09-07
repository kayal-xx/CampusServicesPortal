using CampusServicesPortal.Api.DTOs.Lab;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Repositories;

namespace CampusServicesPortal.Api.Services;

public class LabService
{
    private readonly LabRepository _labRepository;

    public LabService(LabRepository labRepository)
    {
        _labRepository = labRepository;
    }

    public async Task<List<LabDto>> GetLabsAsync()
    {
        var labs = await _labRepository.GetActiveLabsAsync();

        return labs.Select(lab => new LabDto
        {
            Id = lab.Id,
            Name = lab.Name,
            Location = lab.Location,
            Capacity = lab.Capacity,
            IsActive = lab.IsActive
        }).ToList();
    }

    public async Task<LabBookingDto> CreateBookingAsync(
        int studentId,
        CreateLabBookingDto dto)
    {
        var lab = await _labRepository.GetLabByIdAsync(dto.LabId);

        if (lab is null)
        {
            throw new KeyNotFoundException("Lab not found.");
        }

        if (dto.BookingDate.Date < DateTime.Today)
        {
            throw new InvalidOperationException(
                "Booking date cannot be in the past.");
        }

        if (dto.StartTime >= dto.EndTime)
        {
            throw new InvalidOperationException(
                "End time must be later than start time.");
        }

        var hasConflict =
     await _labRepository.HasConflictingBookingAsync(
         dto.LabId,
         dto.BookingDate,
         dto.StartTime,
         dto.EndTime);

        if (hasConflict)
        {
            throw new InvalidOperationException(
                "The lab is already booked for this time period.");
        }

        var booking = new LabBooking
        {
            StudentId = studentId,
            LabId = dto.LabId,
            BookingDate = dto.BookingDate.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = "Booked"
        };

        await _labRepository.AddBookingAsync(booking);

        return ToBookingDto(booking, lab.Name);
    }

    public async Task<List<LabBookingDto>>
        GetStudentBookingsAsync(int studentId)
    {
        var bookings =
            await _labRepository.GetBookingsByStudentAsync(studentId);

        return bookings.Select(booking =>
            ToBookingDto(
                booking,
                booking.Lab?.Name ?? string.Empty,
                booking.Student?.FullName ?? string.Empty
            )).ToList();
    }

    public async Task CancelBookingAsync(
        int bookingId,
        int studentId)
    {
        var booking =
            await _labRepository.GetBookingByIdAsync(bookingId);

        if (booking is null)
        {
            throw new KeyNotFoundException(
                "Lab booking not found.");
        }

        if (booking.StudentId != studentId)
        {
            throw new UnauthorizedAccessException(
                "You cannot cancel another student's booking.");
        }

        if (booking.Status == "Cancelled")
        {
            throw new InvalidOperationException(
                "This booking is already cancelled.");
        }

        booking.Status = "Cancelled";

        await _labRepository.UpdateBookingAsync(booking);
    }

    private static LabBookingDto ToBookingDto(
        LabBooking booking,
        string labName,
        string studentName = "")
    {
        return new LabBookingDto
        {
            Id = booking.Id,
            StudentId = booking.StudentId,
            StudentName = studentName,
            LabId = booking.LabId,
            LabName = labName,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status
        };
    }
}