using CampusServicesPortal.Api.Entities;

namespace CampusServicesPortal.Api.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllAsync();
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetByStudentIdAsync(int studentId);
    Task<Notification> CreateAsync(Notification notification);
    Task<Notification?> UpdateAsync(Notification notification);
}