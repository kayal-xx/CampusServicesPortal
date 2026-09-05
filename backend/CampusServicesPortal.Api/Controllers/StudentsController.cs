using System.Security.Claims;
using CampusServicesPortal.Api.DTOs.Student;
using CampusServicesPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly StudentService _studentService;

    public StudentsController(StudentService studentService)
    {
        _studentService = studentService;
    }

    // Student can view own profile.
    // Admin can view any student profile.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        if (!IsAdmin() && GetLoggedInStudentId() != id)
        {
            return Forbid();
        }

        try
        {
            var student = await _studentService.GetByIdAsync(id);
            return Ok(student);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    // Admin-only student search and filtering.
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<StudentDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] string? faculty)
    {
        var students = await _studentService.SearchAsync(
            search,
            faculty
        );

        return Ok(students);
    }

    // Student can update own profile.
    // Admin can update any student profile.
    [HttpPut("{id:int}")]
    public async Task<ActionResult<StudentDto>> Update(
        int id,
        UpdateStudentDto request)
    {
        if (!IsAdmin() && GetLoggedInStudentId() != id)
        {
            return Forbid();
        }

        try
        {
            var student = await _studentService.UpdateAsync(
                id,
                request
            );

            return Ok(student);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    // Admin-only student deactivation.
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        try
        {
            await _studentService.DeactivateAsync(id);

            return Ok(new
            {
                message = "Student account deactivated successfully."
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    private int? GetLoggedInStudentId()
    {
        var claimValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        return int.TryParse(claimValue, out var studentId)
            ? studentId
            : null;
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }
}