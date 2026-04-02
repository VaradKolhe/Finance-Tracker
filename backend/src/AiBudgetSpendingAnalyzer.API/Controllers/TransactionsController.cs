using AiBudgetSpendingAnalyzer.Application.DTOs.Transactions;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiBudgetSpendingAnalyzer.API.Controllers;

[Authorize]
[Route("api/transactions")]
public class TransactionsController : BaseApiController
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TransactionDto>>> Get(CancellationToken cancellationToken)
    {
        var transactions = await _transactionService.GetTransactionsAsync(CurrentUserId, cancellationToken);
        return Ok(transactions);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionService.CreateTransactionAsync(CurrentUserId, IsAdmin, request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{transactionId:int}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TransactionDto>> Update(int transactionId, [FromBody] UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionService.UpdateTransactionAsync(CurrentUserId, IsAdmin, transactionId, request, cancellationToken);
        return Ok(transaction);
    }

    [HttpDelete("{transactionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int transactionId, CancellationToken cancellationToken)
    {
        await _transactionService.DeleteTransactionAsync(CurrentUserId, transactionId, cancellationToken);
        return NoContent();
    }
}
