using CampusServicesPortal.Api.DTOs.Complaint;

namespace CampusServicesPortal.Api.Interfaces.Services;

public interface IComplaintService
{
    Task<List<ComplaintCategoryDto>>
        GetActiveCategoriesAsync();

    Task<List<ComplaintCategoryDto>>
        GetAllCategoriesAsync();

    Task<(
        bool Success,
        string Message,
        ComplaintCategoryDto? Data
    )> CreateCategoryAsync(
        CreateComplaintCategoryDto dto
    );

    Task<(bool Success, string Message)>
        UpdateCategoryAsync(
            int id,
            UpdateComplaintCategoryDto dto
        );

    Task<List<ComplaintDto>>
        GetAllComplaintsAsync(string? status);

    Task<List<ComplaintDto>>
        GetStudentComplaintsAsync(int studentId);

    Task<ComplaintDto?> GetComplaintByIdAsync(int id);

    Task<(
        bool Success,
        string Message,
        ComplaintDto? Data
    )> CreateComplaintAsync(CreateComplaintDto dto);

    Task<(bool Success, string Message)>
        UpdateComplaintStatusAsync(
            int id,
            UpdateComplaintStatusDto dto
        );
}