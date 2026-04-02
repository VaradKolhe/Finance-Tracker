using AiBudgetSpendingAnalyzer.Application.DTOs.Budgets;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiBudgetSpendingAnalyzer.API.Controllers;

[Authorize]
[Route("api/budgets")]
public class BudgetsController : BaseApiController
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<BudgetProgressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BudgetProgressDto>>> Get([FromQuery] int? year, [FromQuery] int? month, CancellationToken cancellationToken)
    {
        var budgets = await _budgetService.GetBudgetsAsync(CurrentUserId, year, month, cancellationToken);
        return Ok(budgets);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BudgetProgressDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<BudgetProgressDto>> Create([FromBody] CreateBudgetRequest request, CancellationToken cancellationToken)
    {
        var budget = await _budgetService.CreateBudgetAsync(CurrentUserId, IsAdmin, request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = budget.BudgetId }, budget);
    }

    [HttpPut("{budgetId:int}")]
    [ProducesResponseType(typeof(BudgetProgressDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BudgetProgressDto>> Update(int budgetId, [FromBody] UpdateBudgetRequest request, CancellationToken cancellationToken)
    {
        var budget = await _budgetService.UpdateBudgetAsync(CurrentUserId, IsAdmin, budgetId, request, cancellationToken);
        return Ok(budget);
    }

    [HttpDelete("{budgetId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int budgetId, CancellationToken cancellationToken)
    {
        await _budgetService.DeleteBudgetAsync(CurrentUserId, budgetId, cancellationToken);
        return NoContent();
    }
}
