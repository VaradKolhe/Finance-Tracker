using AiBudgetSpendingAnalyzer.Application.DTOs.Transactions;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface ITransactionService
{
    Task<IReadOnlyCollection<TransactionDto>> GetTransactionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateTransactionAsync(int userId, bool isAdmin, CreateTransactionRequest request, CancellationToken cancellationToken = default);
    Task<TransactionDto> UpdateTransactionAsync(int userId, bool isAdmin, int transactionId, UpdateTransactionRequest request, CancellationToken cancellationToken = default);
    Task DeleteTransactionAsync(int userId, int transactionId, CancellationToken cancellationToken = default);
}
