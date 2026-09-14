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
    public async Task<LabDto> CreateLabAsync(CreateLabDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new InvalidOperationException("Lab name is required.");
        }

        if (dto.Capacity < 1)
        {
            throw new InvalidOperationException(
                "Lab capacity must be at least 1.");
        }

        if (await _labRepository.LabNameExistsAsync(dto.Name.Trim()))
        {
            throw new InvalidOperationException(
                "A lab with this name already exists.");
        }

        var lab = new Lab
        {
            Name = dto.Name.Trim(),
            Location = dto.Location.Trim(),
            Capacity = dto.Capacity,
            IsActive = dto.IsActive
        };

        await _labRepository.AddLabAsync(lab);

        return ToLabDto(lab);
    }

    public async Task<LabDto> UpdateLabAsync(
        int labId,
        UpdateLabDto dto)
    {
        var lab = await _labRepository.GetLabForAdminAsync(labId);

        if (lab is null)
        {
            throw new KeyNotFoundException("Lab not found.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new InvalidOperationException("Lab name is required.");
        }

        if (dto.Capacity < 1)
        {
            throw new InvalidOperationException(
                "Lab capacity must be at least 1.");
        }

        if (await _labRepository.LabNameExistsAsync(
            dto.Name.Trim(),
            labId))
        {
            throw new InvalidOperationException(
                "A lab with this name already exists.");
        }

        lab.Name = dto.Name.Trim();
        lab.Location = dto.Location.Trim();
        lab.Capacity = dto.Capacity;
        lab.IsActive = dto.IsActive;

        await _labRepository.UpdateLabAsync(lab);

        return ToLabDto(lab);
    }

    public async Task DeactivateLabAsync(int labId)
    {
        var lab = await _labRepository.GetLabForAdminAsync(labId);

        if (lab is null)
        {
            throw new KeyNotFoundException("Lab not found.");
        }

        lab.IsActive = false;

        await _labRepository.UpdateLabAsync(lab);
    }
    private static LabDto ToLabDto(Lab lab)
    {
        return new LabDto
        {
            Id = lab.Id,
            Name = lab.Name,
            Location = lab.Location,
            Capacity = lab.Capacity,
            IsActive = lab.IsActive
        };
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
            Status = "Pending"
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
    public async Task<List<LabBookingDto>> GetAllBookingsAsync()
    {
        var bookings = await _labRepository.GetAllBookingsAsync();

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
    public async Task<LabBookingDto> UpdateStatusAsync(
    int bookingId,
    UpdateLabBookingStatusDto request)
    {
        var booking =
            await _labRepository.GetBookingByIdAsync(bookingId);

        if (booking is null)
        {
            throw new KeyNotFoundException(
                "Lab booking not found.");
        }

        if (request.Status == "Rejected")
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                throw new InvalidOperationException(
                    "Rejection reason is required.");
            }

            booking.RejectionReason =
                request.RejectionReason.Trim();
        }
        else
        {
            booking.RejectionReason = string.Empty;
        }

        booking.Status = request.Status.Trim();

        await _labRepository.UpdateBookingAsync(booking);

        return ToBookingDto(
            booking,
            booking.Lab?.Name ?? string.Empty,
            booking.Student?.FullName ?? string.Empty
        );
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
            Status = booking.Status,
            RejectionReason = booking.RejectionReason
        };
    }
}