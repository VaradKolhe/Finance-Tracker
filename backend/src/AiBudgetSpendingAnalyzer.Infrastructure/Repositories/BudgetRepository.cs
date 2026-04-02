using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AiBudgetSpendingAnalyzer.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly AppDbContext _dbContext;

    public BudgetRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Budget?> GetByIdAsync(int budgetId, CancellationToken cancellationToken = default) =>
        _dbContext.Budgets
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == budgetId, cancellationToken);

    public Task<Budget?> GetByCategoryAndPeriodAsync(int userId, int categoryId, int year, int month, CancellationToken cancellationToken = default) =>
        _dbContext.Budgets
            .Include(x => x.Category)
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.CategoryId == categoryId && x.Year == year && x.Month == month,
                cancellationToken);

    public async Task<IReadOnlyCollection<Budget>> GetByUserAsync(int userId, int? year = null, int? month = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Budgets
            .Include(x => x.Category)
            .Where(x => x.UserId == userId);

        if (year.HasValue)
        {
            query = query.Where(x => x.Year == year.Value);
        }

        if (month.HasValue)
        {
            query = query.Where(x => x.Month == month.Value);
        }

        return await query.OrderBy(x => x.Category.Name).ToArrayAsync(cancellationToken);
    }

    public Task AddAsync(Budget budget, CancellationToken cancellationToken = default) =>
        _dbContext.Budgets.AddAsync(budget, cancellationToken).AsTask();

    public void Remove(Budget budget) => _dbContext.Budgets.Remove(budget);
}
