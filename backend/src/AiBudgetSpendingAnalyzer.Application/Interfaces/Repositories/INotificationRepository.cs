using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(int notificationId, CancellationToken cancellationToken = default);
    Task<Notification?> GetByReferenceAsync(int userId, string referenceId, NotificationType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Notification>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Notification>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
}
