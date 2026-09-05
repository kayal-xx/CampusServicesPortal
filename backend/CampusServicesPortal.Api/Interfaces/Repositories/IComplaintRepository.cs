using CampusServicesPortal.Api.Entities;

namespace CampusServicesPortal.Api.Interfaces.Repositories;

public interface IComplaintRepository
{
    Task<List<ComplaintCategory>> GetActiveCategoriesAsync();

    Task<List<ComplaintCategory>> GetAllCategoriesAsync();

    Task<ComplaintCategory?> GetCategoryByIdAsync(int id);

    Task<bool> CategoryNameExistsAsync(string name);

    Task<ComplaintCategory> CreateCategoryAsync(
        ComplaintCategory category
    );

    Task<bool> UpdateCategoryAsync(
        ComplaintCategory category
    );

    Task<List<Complaint>> GetAllComplaintsAsync(
        string? status
    );

    Task<List<Complaint>> GetStudentComplaintsAsync(
        int studentId
    );

    Task<Complaint?> GetComplaintByIdAsync(int id);

    Task<Complaint> CreateComplaintAsync(
        Complaint complaint
    );

    Task<bool> UpdateComplaintAsync(
        Complaint complaint
    );
}