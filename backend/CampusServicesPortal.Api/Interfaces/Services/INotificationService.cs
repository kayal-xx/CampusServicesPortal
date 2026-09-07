using CampusServicesPortal.Api.DTOs.Notification;

namespace CampusServicesPortal.Api.Interfaces.Services;

public interface INotificationService
{
    Task<List<NotificationDto>> GetAllAsync();
    Task<NotificationDto?> GetByIdAsync(int id);
    Task<List<NotificationDto>> GetByStudentIdAsync(int studentId);
    Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
    Task<NotificationDto?> UpdateReadStatusAsync(
        int id,
        UpdateNotificationReadStatusDto dto
    );
}