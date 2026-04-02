using AiBudgetSpendingAnalyzer.Domain.Entities;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(int budgetId, CancellationToken cancellationToken = default);
    Task<Budget?> GetByCategoryAndPeriodAsync(int userId, int categoryId, int year, int month, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Budget>> GetByUserAsync(int userId, int? year = null, int? month = null, CancellationToken cancellationToken = default);
    Task AddAsync(Budget budget, CancellationToken cancellationToken = default);
    void Remove(Budget budget);
}
