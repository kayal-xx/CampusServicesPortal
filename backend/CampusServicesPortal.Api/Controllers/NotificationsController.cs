using CampusServicesPortal.Api.DTOs.Notification;
using CampusServicesPortal.Api.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetAll()
    {
        var notifications = await _notificationService.GetAllAsync();
        return Ok(notifications);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NotificationDto>> GetById(int id)
    {
        var notification = await _notificationService.GetByIdAsync(id);

        if (notification == null)
        {
            return NotFound();
        }

        return Ok(notification);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<ActionResult<List<NotificationDto>>> GetByStudentId(int studentId)
    {
        var notifications =
            await _notificationService.GetByStudentIdAsync(studentId);

        return Ok(notifications);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create(CreateNotificationDto dto)
    {
        var createdNotification =
            await _notificationService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdNotification.Id },
            createdNotification
        );
    }

    [HttpPut("{id:int}/read-status")]
    public async Task<ActionResult<NotificationDto>> UpdateReadStatus(
        int id,
        UpdateNotificationReadStatusDto dto)
    {
        var updatedNotification =
            await _notificationService.UpdateReadStatusAsync(id, dto);

        if (updatedNotification == null)
        {
            return NotFound();
        }

        return Ok(updatedNotification);
    }
}