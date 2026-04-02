using AiBudgetSpendingAnalyzer.Application.DTOs.Notifications;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiBudgetSpendingAnalyzer.API.Controllers;

[Authorize]
[Route("api/notifications")]
public class NotificationsController : BaseApiController
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<NotificationDto>>> Get(CancellationToken cancellationToken)
    {
        await _notificationService.EvaluateNotificationsAsync(CurrentUserId, cancellationToken: cancellationToken);
        var notifications = await _notificationService.GetNotificationsAsync(CurrentUserId, cancellationToken);
        return Ok(notifications);
    }

    [HttpPut("{notificationId:int}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsRead(int notificationId, CancellationToken cancellationToken)
    {
        await _notificationService.MarkAsReadAsync(CurrentUserId, notificationId, cancellationToken);
        return NoContent();
    }
}
