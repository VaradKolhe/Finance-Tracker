using AiBudgetSpendingAnalyzer.Domain.Entities;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(int transactionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> GetByUserAsync(
        int userId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> GetRecentAsync(int userId, int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> GetAllAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken cancellationToken = default);
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    void Remove(Transaction transaction);
}
