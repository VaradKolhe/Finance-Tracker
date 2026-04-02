using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Domain.Enums;
using AiBudgetSpendingAnalyzer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AiBudgetSpendingAnalyzer.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _dbContext;

    public NotificationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Notification?> GetByIdAsync(int notificationId, CancellationToken cancellationToken = default) =>
        _dbContext.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId, cancellationToken);

    public Task<Notification?> GetByReferenceAsync(int userId, string referenceId, NotificationType type, CancellationToken cancellationToken = default) =>
        _dbContext.Notifications.FirstOrDefaultAsync(
            x => x.UserId == userId && x.ReferenceId == referenceId && x.Type == type,
            cancellationToken);

    public async Task<IReadOnlyCollection<Notification>> GetByUserAsync(int userId, CancellationToken cancellationToken = default) =>
        await _dbContext.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Notification>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Notifications.AsNoTracking().ToArrayAsync(cancellationToken);

    public Task AddAsync(Notification notification, CancellationToken cancellationToken = default) =>
        _dbContext.Notifications.AddAsync(notification, cancellationToken).AsTask();
}
