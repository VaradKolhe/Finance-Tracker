using AiBudgetSpendingAnalyzer.Application.DTOs.Reports;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiBudgetSpendingAnalyzer.API.Controllers;

[Authorize]
[Route("api/reports")]
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportDto>> Generate([FromBody] ReportRequestDto request, CancellationToken cancellationToken)
    {
        var report = await _reportService.GenerateAsync(CurrentUserId, request, cancellationToken);
        return Ok(report);
    }
}
