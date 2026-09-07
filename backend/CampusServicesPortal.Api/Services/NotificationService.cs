using CampusServicesPortal.Api.DTOs.Notification;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using CampusServicesPortal.Api.Interfaces.Services;

namespace CampusServicesPortal.Api.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDto>> GetAllAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();

        return notifications
            .Select(MapToDto)
            .ToList();
    }

    public async Task<NotificationDto?> GetByIdAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);

        return notification == null
            ? null
            : MapToDto(notification);
    }

    public async Task<List<NotificationDto>> GetByStudentIdAsync(int studentId)
    {
        var notifications =
            await _notificationRepository.GetByStudentIdAsync(studentId);

        return notifications
            .Select(MapToDto)
            .ToList();
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            StudentId = dto.StudentId,
            Message = dto.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdNotification =
            await _notificationRepository.CreateAsync(notification);

        return MapToDto(createdNotification);
    }

    public async Task<NotificationDto?> UpdateReadStatusAsync(
        int id,
        UpdateNotificationReadStatusDto dto)
    {
        var notification =
            await _notificationRepository.GetByIdAsync(id);

        if (notification == null)
        {
            return null;
        }

        notification.IsRead = dto.IsRead;

        var updatedNotification =
            await _notificationRepository.UpdateAsync(notification);

        return updatedNotification == null
            ? null
            : MapToDto(updatedNotification);
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            StudentId = notification.StudentId,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}