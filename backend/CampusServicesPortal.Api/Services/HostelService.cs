using CampusServicesPortal.Api.DTOs.Hostel;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Repositories;

namespace CampusServicesPortal.Api.Services;

public class HostelService
{
    private readonly HostelRepository _hostelRepository;

    public HostelService(HostelRepository hostelRepository)
    {
        _hostelRepository = hostelRepository;
    }

    public async Task<List<HostelDto>> GetHostelsAsync()
    {
        var hostels =
            await _hostelRepository.GetActiveHostelsAsync();

        return hostels.Select(hostel => new HostelDto
        {
            Id = hostel.Id,
            Name = hostel.Name,
            Location = hostel.Location,
            IsActive = hostel.IsActive
        }).ToList();
    }

    public async Task<List<RoomDto>> GetRoomsByHostelAsync(
        int hostelId)
    {
        var hostel =
            await _hostelRepository.GetHostelByIdAsync(hostelId);

        if (hostel is null)
        {
            throw new KeyNotFoundException("Hostel not found.");
        }

        var rooms =
            await _hostelRepository.GetRoomsByHostelAsync(hostelId);

        var roomDtos = new List<RoomDto>();

        foreach (var room in rooms)
        {
            var occupancy =
                await _hostelRepository.GetRoomOccupancyAsync(
                    room.Id
                );

            roomDtos.Add(new RoomDto
            {
                Id = room.Id,
                HostelId = room.HostelId,
                HostelName = room.Hostel?.Name ?? hostel.Name,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                CurrentOccupancy = occupancy
            });
        }

        return roomDtos;
    }

    public async Task<HostelApplicationDto> CreateApplicationAsync(
        int studentId,
        CreateHostelApplicationDto request)
    {
        var hostel =
            await _hostelRepository.GetHostelByIdAsync(
                request.HostelId
            );

        if (hostel is null)
        {
            throw new KeyNotFoundException("Hostel not found.");
        }

        var application = new HostelApplication
        {
            StudentId = studentId,
            HostelId = request.HostelId,
            Semester = request.Semester.Trim(),
            SpecialRequirements =
                request.SpecialRequirements.Trim(),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _hostelRepository.AddApplicationAsync(application);

        var savedApplication =
            await _hostelRepository.GetApplicationByIdAsync(
                application.Id
            );

        return MapApplicationToDto(
            savedApplication ?? application
        );
    }

    public async Task<List<HostelApplicationDto>>
        GetStudentApplicationsAsync(int studentId)
    {
        var applications =
            await _hostelRepository
                .GetApplicationsByStudentAsync(studentId);

        return applications
            .Select(MapApplicationToDto)
            .ToList();
    }

    public async Task<List<HostelApplicationDto>>
        GetAllApplicationsAsync(string? status)
    {
        var applications =
            await _hostelRepository.GetApplicationsAsync(status);

        return applications
            .Select(MapApplicationToDto)
            .ToList();
    }

    public async Task<HostelApplicationDto> UpdateStatusAsync(
        int applicationId,
        UpdateHostelApplicationStatusDto request)
    {
        var application =
            await _hostelRepository.GetApplicationByIdAsync(
                applicationId
            );

        if (application is null)
        {
            throw new KeyNotFoundException(
                "Hostel application not found."
            );
        }

        if (application.Status == "Room Assigned")
        {
            throw new InvalidOperationException(
                "A room-assigned application status cannot be changed."
            );
        }

        application.Status = request.Status.Trim();
        application.RoomId = null;

        await _hostelRepository.UpdateApplicationAsync(application);

        return MapApplicationToDto(application);
    }

    public async Task<HostelApplicationDto> AssignRoomAsync(
        int applicationId,
        AssignRoomDto request)
    {
        var application =
            await _hostelRepository.GetApplicationByIdAsync(
                applicationId
            );

        if (application is null)
        {
            throw new KeyNotFoundException(
                "Hostel application not found."
            );
        }

        if (application.Status != "Approved")
        {
            throw new InvalidOperationException(
                "The application must be approved before assigning a room."
            );
        }

        var room =
            await _hostelRepository.GetRoomByIdAsync(
                request.RoomId
            );

        if (room is null)
        {
            throw new KeyNotFoundException("Room not found.");
        }

        if (room.HostelId != application.HostelId)
        {
            throw new InvalidOperationException(
                "The selected room does not belong to the requested hostel."
            );
        }

        var occupancy =
            await _hostelRepository.GetRoomOccupancyAsync(room.Id);

        if (occupancy >= room.Capacity)
        {
            throw new InvalidOperationException(
                "The selected room has reached its capacity."
            );
        }

        application.RoomId = room.Id;
        application.Room = room;
        application.Status = "Room Assigned";

        await _hostelRepository.UpdateApplicationAsync(application);

        return MapApplicationToDto(application);
    }
    public async Task<HostelDto> CreateHostelAsync(
        CreateHostelDto request)
    {
        var nameExists =
            await _hostelRepository.HostelNameExistsAsync(
                request.Name
            );

        if (nameExists)
        {
            throw new InvalidOperationException(
                "A hostel with this name already exists."
            );
        }

        var hostel = new Hostel
        {
            Name = request.Name.Trim(),
            Location = request.Location.Trim(),
            IsActive = true
        };

        await _hostelRepository.AddHostelAsync(hostel);

        return new HostelDto
        {
            Id = hostel.Id,
            Name = hostel.Name,
            Location = hostel.Location,
            IsActive = hostel.IsActive
        };
    }

    public async Task<RoomDto> CreateRoomAsync(
        CreateRoomDto request)
    {
        var hostel =
            await _hostelRepository.GetHostelByIdAsync(
                request.HostelId
            );

        if (hostel is null)
        {
            throw new KeyNotFoundException("Hostel not found.");
        }

        var roomNumberExists =
            await _hostelRepository.RoomNumberExistsAsync(
                request.HostelId,
                request.RoomNumber
            );

        if (roomNumberExists)
        {
            throw new InvalidOperationException(
                "This room number already exists in the hostel."
            );
        }

        var room = new Room
        {
            HostelId = request.HostelId,
            RoomNumber = request.RoomNumber.Trim(),
            Capacity = request.Capacity,
            Hostel = hostel
        };

        await _hostelRepository.AddRoomAsync(room);

        return new RoomDto
        {
            Id = room.Id,
            HostelId = room.HostelId,
            HostelName = hostel.Name,
            RoomNumber = room.RoomNumber,
            Capacity = room.Capacity,
            CurrentOccupancy = 0
        };
    }
    public async Task<HostelDto> UpdateHostelAsync(
    int hostelId,
    UpdateHostelDto request)
    {
        var hostel =
            await _hostelRepository.GetHostelByIdAsync(hostelId);

        if (hostel is null)
        {
            throw new KeyNotFoundException("Hostel not found.");
        }

        var nameExists =
            await _hostelRepository
                .HostelNameExistsForOtherHostelAsync(
                    request.Name,
                    hostelId
                );

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Another hostel already uses this name."
            );
        }

        hostel.Name = request.Name.Trim();
        hostel.Location = request.Location.Trim();

        await _hostelRepository.UpdateHostelAsync(hostel);

        return new HostelDto
        {
            Id = hostel.Id,
            Name = hostel.Name,
            Location = hostel.Location,
            IsActive = hostel.IsActive
        };
    }

    public async Task DeactivateHostelAsync(int hostelId)
    {
        var hostel =
            await _hostelRepository.GetHostelByIdAsync(hostelId);

        if (hostel is null)
        {
            throw new KeyNotFoundException("Hostel not found.");
        }

        hostel.IsActive = false;

        await _hostelRepository.UpdateHostelAsync(hostel);
    }

    public async Task<RoomDto> UpdateRoomAsync(
        int roomId,
        UpdateRoomDto request)
    {
        var room =
            await _hostelRepository.GetRoomByIdAsync(roomId);

        if (room is null)
        {
            throw new KeyNotFoundException("Room not found.");
        }

        var roomNumberExists =
            await _hostelRepository
                .RoomNumberExistsForOtherRoomAsync(
                    room.HostelId,
                    request.RoomNumber,
                    roomId
                );

        if (roomNumberExists)
        {
            throw new InvalidOperationException(
                "Another room in this hostel already uses this number."
            );
        }

        var currentOccupancy =
            await _hostelRepository.GetRoomOccupancyAsync(roomId);

        if (request.Capacity < currentOccupancy)
        {
            throw new InvalidOperationException(
                "Room capacity cannot be lower than current occupancy."
            );
        }

        room.RoomNumber = request.RoomNumber.Trim();
        room.Capacity = request.Capacity;

        await _hostelRepository.UpdateRoomAsync(room);

        return new RoomDto
        {
            Id = room.Id,
            HostelId = room.HostelId,
            HostelName = room.Hostel?.Name ?? string.Empty,
            RoomNumber = room.RoomNumber,
            Capacity = room.Capacity,
            CurrentOccupancy = currentOccupancy
        };
    }
    private static HostelApplicationDto MapApplicationToDto(
        HostelApplication application)
    {
        return new HostelApplicationDto
        {
            Id = application.Id,
            StudentId = application.StudentId,
            StudentName =
                application.Student?.FullName ?? string.Empty,
            HostelId = application.HostelId,
            HostelName =
                application.Hostel?.Name ?? string.Empty,
            RoomId = application.RoomId,
            RoomNumber = application.Room?.RoomNumber,
            Semester = application.Semester,
            SpecialRequirements =
                application.SpecialRequirements,
            Status = application.Status,
            CreatedAt = application.CreatedAt
        };
    }
}