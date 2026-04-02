using AiBudgetSpendingAnalyzer.Application.DTOs.Budgets;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface IBudgetService
{
    Task<IReadOnlyCollection<BudgetProgressDto>> GetBudgetsAsync(int userId, int? year = null, int? month = null, CancellationToken cancellationToken = default);
    Task<BudgetProgressDto> CreateBudgetAsync(int userId, bool isAdmin, CreateBudgetRequest request, CancellationToken cancellationToken = default);
    Task<BudgetProgressDto> UpdateBudgetAsync(int userId, bool isAdmin, int budgetId, UpdateBudgetRequest request, CancellationToken cancellationToken = default);
    Task DeleteBudgetAsync(int userId, int budgetId, CancellationToken cancellationToken = default);
}
