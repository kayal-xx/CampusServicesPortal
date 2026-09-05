using CampusServicesPortal.Api.DTOs.Complaint;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Interfaces.Repositories;
using CampusServicesPortal.Api.Interfaces.Services;

namespace CampusServicesPortal.Api.Services;

public class ComplaintService : IComplaintService
{
    private readonly IComplaintRepository _complaintRepository;

    private static readonly string[] AllowedStatuses =
    {
        "Pending",
        "In Progress",
        "Resolved"
    };

    public ComplaintService(
        IComplaintRepository complaintRepository
    )
    {
        _complaintRepository = complaintRepository;
    }

    public async Task<List<ComplaintCategoryDto>>
        GetActiveCategoriesAsync()
    {
        List<ComplaintCategory> categories =
            await _complaintRepository.GetActiveCategoriesAsync();

        return categories.Select(MapCategory).ToList();
    }

    public async Task<List<ComplaintCategoryDto>>
        GetAllCategoriesAsync()
    {
        List<ComplaintCategory> categories =
            await _complaintRepository.GetAllCategoriesAsync();

        return categories.Select(MapCategory).ToList();
    }

    public async Task<(
        bool Success,
        string Message,
        ComplaintCategoryDto? Data
    )> CreateCategoryAsync(CreateComplaintCategoryDto dto)
    {
        string categoryName = dto.Name.Trim();

        bool exists =
            await _complaintRepository
                .CategoryNameExistsAsync(categoryName);

        if (exists)
        {
            return (
                false,
                "Complaint category already exists.",
                null
            );
        }

        ComplaintCategory category = new()
        {
            Name = categoryName,
            IsActive = true
        };

        ComplaintCategory createdCategory =
            await _complaintRepository
                .CreateCategoryAsync(category);

        return (
            true,
            "Complaint category created successfully.",
            MapCategory(createdCategory)
        );
    }

    public async Task<(bool Success, string Message)>
        UpdateCategoryAsync(
            int id,
            UpdateComplaintCategoryDto dto
        )
    {
        ComplaintCategory? category =
            await _complaintRepository
                .GetCategoryByIdAsync(id);

        if (category is null)
        {
            return (
                false,
                "Complaint category not found."
            );
        }

        string categoryName = dto.Name.Trim();

        if (!category.Name.Equals(
                categoryName,
                StringComparison.OrdinalIgnoreCase))
        {
            bool nameExists =
                await _complaintRepository
                    .CategoryNameExistsAsync(categoryName);

            if (nameExists)
            {
                return (
                    false,
                    "Complaint category name already exists."
                );
            }
        }

        category.Name = categoryName;
        category.IsActive = dto.IsActive;

        await _complaintRepository
            .UpdateCategoryAsync(category);

        return (
            true,
            "Complaint category updated successfully."
        );
    }

    public async Task<List<ComplaintDto>>
        GetAllComplaintsAsync(string? status)
    {
        List<Complaint> complaints =
            await _complaintRepository
                .GetAllComplaintsAsync(status);

        return complaints.Select(MapComplaint).ToList();
    }

    public async Task<List<ComplaintDto>>
        GetStudentComplaintsAsync(int studentId)
    {
        List<Complaint> complaints =
            await _complaintRepository
                .GetStudentComplaintsAsync(studentId);

        return complaints.Select(MapComplaint).ToList();
    }

    public async Task<ComplaintDto?>
        GetComplaintByIdAsync(int id)
    {
        Complaint? complaint =
            await _complaintRepository
                .GetComplaintByIdAsync(id);

        return complaint is null
            ? null
            : MapComplaint(complaint);
    }

    public async Task<(
        bool Success,
        string Message,
        ComplaintDto? Data
    )> CreateComplaintAsync(CreateComplaintDto dto)
    {
        ComplaintCategory? category =
            await _complaintRepository
                .GetCategoryByIdAsync(
                    dto.ComplaintCategoryId
                );

        if (category is null || !category.IsActive)
        {
            return (
                false,
                "A valid active complaint category is required.",
                null
            );
        }

        Complaint complaint = new()
        {
            StudentId = dto.StudentId,
            ComplaintCategoryId =
                dto.ComplaintCategoryId,
            Description = dto.Description.Trim(),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            ComplaintCategory = category
        };

        Complaint createdComplaint =
            await _complaintRepository
                .CreateComplaintAsync(complaint);

        return (
            true,
            "Complaint submitted successfully.",
            MapComplaint(createdComplaint)
        );
    }

    public async Task<(bool Success, string Message)>
        UpdateComplaintStatusAsync(
            int id,
            UpdateComplaintStatusDto dto
        )
    {
        Complaint? complaint =
            await _complaintRepository
                .GetComplaintByIdAsync(id);

        if (complaint is null)
        {
            return (
                false,
                "Complaint not found."
            );
        }

        string? validStatus =
            AllowedStatuses.FirstOrDefault(
                status => status.Equals(
                    dto.Status.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            );

        if (validStatus is null)
        {
            return (
                false,
                "Status must be Pending, In Progress, or Resolved."
            );
        }

        if (validStatus == "Resolved" &&
            string.IsNullOrWhiteSpace(dto.ResolutionNote))
        {
            return (
                false,
                "A resolution note is required when resolving a complaint."
            );
        }

        complaint.Status = validStatus;
        complaint.ResolutionNote =
            string.IsNullOrWhiteSpace(dto.ResolutionNote)
                ? null
                : dto.ResolutionNote.Trim();

        await _complaintRepository
            .UpdateComplaintAsync(complaint);

        return (
            true,
            "Complaint status updated successfully."
        );
    }

    private static ComplaintCategoryDto MapCategory(
        ComplaintCategory category
    )
    {
        return new ComplaintCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            IsActive = category.IsActive
        };
    }

    private static ComplaintDto MapComplaint(
        Complaint complaint
    )
    {
        return new ComplaintDto
        {
            Id = complaint.Id,
            StudentId = complaint.StudentId,
            ComplaintCategoryId =
                complaint.ComplaintCategoryId,
            CategoryName =
                complaint.ComplaintCategory?.Name
                ?? string.Empty,
            Description = complaint.Description,
            Status = complaint.Status,
            ResolutionNote = complaint.ResolutionNote,
            CreatedAt = complaint.CreatedAt
        };
    }
}