using AiBudgetSpendingAnalyzer.Application.DTOs.Notifications;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface INotificationService
{
    Task<IReadOnlyCollection<NotificationDto>> GetNotificationsAsync(int userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default);
    Task EvaluateNotificationsAsync(int userId, int? transactionId = null, CancellationToken cancellationToken = default);
}
