using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AiBudgetSpendingAnalyzer.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _dbContext;

    public TransactionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Transaction?> GetByIdAsync(int transactionId, CancellationToken cancellationToken = default) =>
        _dbContext.Transactions
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == transactionId, cancellationToken);

    public async Task<IReadOnlyCollection<Transaction>> GetByUserAsync(
        int userId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Transactions
            .Include(x => x.Category)
            .Where(x => x.UserId == userId);

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.TransactionDateUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(x => x.TransactionDateUtc < toUtc.Value);
        }

        return await query
            .OrderByDescending(x => x.TransactionDateUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Transaction>> GetRecentAsync(int userId, int count, CancellationToken cancellationToken = default) =>
        await _dbContext.Transactions
            .Include(x => x.Category)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.TransactionDateUtc)
            .Take(count)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Transaction>> GetAllAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Transactions.Include(x => x.Category).AsQueryable();

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.TransactionDateUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(x => x.TransactionDateUtc < toUtc.Value);
        }

        return await query.ToArrayAsync(cancellationToken);
    }

    public Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default) =>
        _dbContext.Transactions.AddAsync(transaction, cancellationToken).AsTask();

    public void Remove(Transaction transaction) => _dbContext.Transactions.Remove(transaction);
}
