using CampusServicesPortal.Api.DTOs.Fee;
using CampusServicesPortal.Api.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeePaymentsController : ControllerBase
{
    private readonly IFeeService _feeService;

    public FeePaymentsController(IFeeService feeService)
    {
        _feeService = feeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FeePaymentDto>>> GetAll()
    {
        var fees = await _feeService.GetAllAsync();
        return Ok(fees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FeePaymentDto>> GetById(int id)
    {
        var fee = await _feeService.GetByIdAsync(id);

        if (fee == null)
        {
            return NotFound();
        }

        return Ok(fee);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<List<FeePaymentDto>>> GetByStudentId(int studentId)
    {
        var fees = await _feeService.GetByStudentIdAsync(studentId);
        return Ok(fees);
    }

    [HttpPost]
    public async Task<ActionResult<FeePaymentDto>> Create(CreateFeePaymentDto dto)
    {
        try
        {
            var createdFee = await _feeService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdFee.Id },
                createdFee
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<FeePaymentDto>> UpdateStatus(
        int id,
        UpdateFeePaymentStatusDto dto)
    {
        var updatedFee = await _feeService.UpdateStatusAsync(id, dto);

        if (updatedFee == null)
        {
            return NotFound();
        }

        return Ok(updatedFee);
    }
}