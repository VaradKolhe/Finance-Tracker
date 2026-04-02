using AiBudgetSpendingAnalyzer.Application.DTOs.AI;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiBudgetSpendingAnalyzer.API.Controllers;

[Authorize]
[Route("api/ai-analysis")]
public class AiAnalysisController : BaseApiController
{
    private readonly ISpendingAnalysisService _spendingAnalysisService;

    public AiAnalysisController(ISpendingAnalysisService spendingAnalysisService)
    {
        _spendingAnalysisService = spendingAnalysisService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(AiAnalysisDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AiAnalysisDto>> Get(CancellationToken cancellationToken)
    {
        var analysis = await _spendingAnalysisService.AnalyzeAsync(CurrentUserId, cancellationToken);
        return Ok(analysis);
    }
}
