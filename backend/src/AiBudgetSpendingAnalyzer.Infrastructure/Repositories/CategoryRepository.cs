using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AiBudgetSpendingAnalyzer.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Category?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default) =>
        _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == categoryId, cancellationToken);

    public async Task<IReadOnlyCollection<Category>> GetAccessibleByUserAsync(int userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Categories.AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.IsSystemDefined || x.UserId == userId);
        }

        return await query
            .AsNoTracking()
            .OrderByDescending(x => x.IsSystemDefined)
            .ThenBy(x => x.Name)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Categories.AsNoTracking().OrderBy(x => x.Name).ToArrayAsync(cancellationToken);

    public async Task<bool> IsInUseAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Transactions.AnyAsync(x => x.CategoryId == categoryId, cancellationToken)
            || await _dbContext.Budgets.AnyAsync(x => x.CategoryId == categoryId, cancellationToken);
    }

    public Task AddAsync(Category category, CancellationToken cancellationToken = default) =>
        _dbContext.Categories.AddAsync(category, cancellationToken).AsTask();

    public void Remove(Category category) => _dbContext.Categories.Remove(category);
}
