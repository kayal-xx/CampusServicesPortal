using CampusServicesPortal.Api.DTOs.Lab;
using CampusServicesPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/labs")]
[Authorize]
public class LabsController : ControllerBase
{
    private readonly LabService _labService;

    public LabsController(LabService labService)
    {
        _labService = labService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLabs()
    {
        var labs = await _labService.GetLabsAsync();

        return Ok(labs);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLab(
        [FromBody] CreateLabDto dto)
    {
        try
        {
            var lab = await _labService.CreateLabAsync(dto);

            return Ok(lab);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLab(
        int id,
        [FromBody] UpdateLabDto dto)
    {
        try
        {
            var lab = await _labService.UpdateLabAsync(id, dto);

            return Ok(lab);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateLab(int id)
    {
        try
        {
            await _labService.DeactivateLabAsync(id);

            return Ok(new
            {
                message = "Lab deactivated successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}