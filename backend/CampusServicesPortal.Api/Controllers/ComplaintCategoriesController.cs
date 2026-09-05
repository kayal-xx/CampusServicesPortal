using CampusServicesPortal.Api.DTOs.Complaint;
using CampusServicesPortal.Api.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/complaint-categories")]
public class ComplaintCategoriesController : ControllerBase
{
    private readonly IComplaintService _complaintService;

    public ComplaintCategoriesController(
        IComplaintService complaintService
    )
    {
        _complaintService = complaintService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ComplaintCategoryDto>>>
        GetActiveCategories()
    {
        return Ok(
            await _complaintService
                .GetActiveCategoriesAsync()
        );
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<ComplaintCategoryDto>>>
        GetAllCategories()
    {
        return Ok(
            await _complaintService
                .GetAllCategoriesAsync()
        );
    }

    [HttpPost]
    public async Task<ActionResult<ComplaintCategoryDto>>
        Create(CreateComplaintCategoryDto dto)
    {
        var result =
            await _complaintService.CreateCategoryAsync(dto);

        if (!result.Success)
        {
            return Conflict(new
            {
                message = result.Message
            });
        }

        return Created(
            $"/api/complaint-categories/{result.Data!.Id}",
            result.Data
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateComplaintCategoryDto dto
    )
    {
        var result =
            await _complaintService
                .UpdateCategoryAsync(id, dto);

        if (!result.Success)
        {
            if (result.Message ==
                "Complaint category not found.")
            {
                return NotFound(new
                {
                    message = result.Message
                });
            }

            return Conflict(new
            {
                message = result.Message
            });
        }

        return NoContent();
    }
}