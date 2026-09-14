using CampusServicesPortal.Api.DTOs.Certificate;
using CampusServicesPortal.Api.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/certificate-requests")]
public class CertificateRequestsController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificateRequestsController(
        ICertificateService certificateService
    )
    {
        _certificateService = certificateService;
    }

    [HttpPost]
    public async Task<ActionResult<List<CertificateRequestDto>>>
        Create(CreateCertificateRequestDto dto)
    {
        var result =
            await _certificateService.CreateAsync(dto);

        if (!result.Success)
        {
            if (result.Message.Contains(
                    "pending request",
                    StringComparison.OrdinalIgnoreCase) ||
                result.Message.Contains(
                    "cannot be requested twice",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new
                {
                    message = result.Message
                });
            }

            return BadRequest(new
            {
                message = result.Message
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.Data
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<CertificateRequestDto>>>
        GetAll([FromQuery] string? status)
    {
        return Ok(
            await _certificateService.GetAllAsync(status)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CertificateRequestDto>>
        GetById(int id)
    {
        CertificateRequestDto? request =
            await _certificateService.GetByIdAsync(id);

        if (request is null)
        {
            return NotFound(new
            {
                message = "Certificate request not found."
            });
        }

        return Ok(request);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<List<CertificateRequestDto>>>
        GetByStudent(int studentId)
    {
        return Ok(
            await _certificateService
                .GetByStudentAsync(studentId)
        );
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateCertificateRequestStatusDto dto
    )
    {
        var result =
            await _certificateService
                .UpdateStatusAsync(id, dto);

        if (!result.Success)
        {
            if (result.Message ==
                "Certificate request not found.")
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