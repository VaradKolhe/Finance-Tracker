using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AiBudgetSpendingAnalyzer.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default) =>
        _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Users.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).ToArrayAsync(cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        _dbContext.Users.AddAsync(user, cancellationToken).AsTask();
}
