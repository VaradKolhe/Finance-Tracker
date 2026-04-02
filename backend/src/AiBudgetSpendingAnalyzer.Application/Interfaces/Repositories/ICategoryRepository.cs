using AiBudgetSpendingAnalyzer.Domain.Entities;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Category>> GetAccessibleByUserAsync(int userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> IsInUseAsync(int categoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    void Remove(Category category);
}
