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
}