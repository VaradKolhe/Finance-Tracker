using AiBudgetSpendingAnalyzer.Application.DTOs.Dashboard;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiBudgetSpendingAnalyzer.API.Controllers;

[Authorize]
[Route("api/dashboard")]
public class DashboardController : BaseApiController
{
    private readonly IDashboardService _dashboardService;
    private readonly INotificationService _notificationService;

    public DashboardController(IDashboardService dashboardService, INotificationService notificationService)
    {
        _dashboardService = dashboardService;
        _notificationService = notificationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> Get([FromQuery] int? year, [FromQuery] int? month, CancellationToken cancellationToken)
    {
        await _notificationService.EvaluateNotificationsAsync(CurrentUserId, cancellationToken: cancellationToken);
        var dashboard = await _dashboardService.GetDashboardAsync(CurrentUserId, year, month, cancellationToken);
        return Ok(dashboard);
    }
}
