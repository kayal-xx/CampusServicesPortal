using CampusServicesPortal.Api.DTOs.Complaint;
using CampusServicesPortal.Api.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/complaints")]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _complaintService;

    public ComplaintsController(
        IComplaintService complaintService
    )
    {
        _complaintService = complaintService;
    }

    [HttpPost]
    public async Task<ActionResult<ComplaintDto>>
        Create(CreateComplaintDto dto)
    {
        var result =
            await _complaintService
                .CreateComplaintAsync(dto);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Created(
            $"/api/complaints/{result.Data!.Id}",
            result.Data
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<ComplaintDto>>>
        GetAll([FromQuery] string? status)
    {
        return Ok(
            await _complaintService
                .GetAllComplaintsAsync(status)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ComplaintDto>>
        GetById(int id)
    {
        ComplaintDto? complaint =
            await _complaintService
                .GetComplaintByIdAsync(id);

        if (complaint is null)
        {
            return NotFound(new
            {
                message = "Complaint not found."
            });
        }

        return Ok(complaint);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<List<ComplaintDto>>>
        GetStudentComplaints(int studentId)
    {
        return Ok(
            await _complaintService
                .GetStudentComplaintsAsync(studentId)
        );
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateComplaintStatusDto dto
    )
    {
        var result =
            await _complaintService
                .UpdateComplaintStatusAsync(id, dto);

        if (!result.Success)
        {
            if (result.Message == "Complaint not found.")
            {
                return NotFound(new
                {
                    message = result.Message
                });
            }

            return BadRequest(new
            {
                message = result.Message
            });
        }

        return NoContent();
    }
}