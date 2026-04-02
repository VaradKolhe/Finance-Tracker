using AiBudgetSpendingAnalyzer.Application.Common.Exceptions;
using AiBudgetSpendingAnalyzer.Application.Common.Mappings;
using AiBudgetSpendingAnalyzer.Application.DTOs.Notifications;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationRepository notificationRepository,
        ITransactionRepository transactionRepository,
        IBudgetRepository budgetRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _transactionRepository = transactionRepository;
        _budgetRepository = budgetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<NotificationDto>> GetNotificationsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetByUserAsync(userId, cancellationToken);

        return notifications
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(20)
            .Select(x => x.ToNotificationDto())
            .ToArray();
    }

    public async Task MarkAsReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken)
            ?? throw new NotFoundException("Notification was not found.");

        if (notification.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to modify this notification.");
        }

        notification.IsRead = true;
        notification.UpdatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task EvaluateNotificationsAsync(int userId, int? transactionId = null, CancellationToken cancellationToken = default)
    {
        if (transactionId.HasValue)
        {
            await EvaluateUnusualSpendingAsync(userId, transactionId.Value, cancellationToken);
        }

        await EvaluateBudgetThresholdsAsync(userId, transactionId, cancellationToken);
    }

    private async Task EvaluateUnusualSpendingAsync(int userId, int transactionId, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken);
        if (transaction is null || transaction.UserId != userId || transaction.Type != TransactionType.Expense)
        {
            return;
        }

        var referenceId = $"transaction:{transaction.Id}:unusual";
        var existing = await _notificationRepository.GetByReferenceAsync(userId, referenceId, NotificationType.UnusualSpending, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var lookbackStart = transaction.TransactionDateUtc.AddDays(-30);
        var expenseHistory = await _transactionRepository.GetByUserAsync(userId, lookbackStart, transaction.TransactionDateUtc, cancellationToken);
        var previousExpenses = expenseHistory
            .Where(x => x.Type == TransactionType.Expense && x.Id != transaction.Id)
            .ToArray();

        if (previousExpenses.Length < 3)
        {
            return;
        }

        var averageAmount = previousExpenses.Average(x => x.Amount);
        if (averageAmount <= 0 || transaction.Amount < averageAmount * 1.75m)
        {
            return;
        }

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = userId,
            Type = NotificationType.UnusualSpending,
            Title = "Unusual spending detected",
            Message = $"A recent expense of {transaction.Amount:C} in {transaction.Category.Name} is significantly above your recent average.",
            ReferenceId = referenceId
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EvaluateBudgetThresholdsAsync(int userId, int? transactionId, CancellationToken cancellationToken)
    {
        var anchorDate = DateTime.UtcNow;
        if (transactionId.HasValue)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId.Value, cancellationToken);
            if (transaction?.UserId == userId)
            {
                anchorDate = transaction.TransactionDateUtc;
            }
        }

        var budgets = await _budgetRepository.GetByUserAsync(userId, anchorDate.Year, anchorDate.Month, cancellationToken);
        if (!budgets.Any())
        {
            return;
        }

        var periodStart = new DateTime(anchorDate.Year, anchorDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1);
        var monthTransactions = await _transactionRepository.GetByUserAsync(userId, periodStart, periodEnd, cancellationToken);
        var changed = false;

        foreach (var budget in budgets)
        {
            var spentAmount = monthTransactions
                .Where(x => x.Type == TransactionType.Expense && x.CategoryId == budget.CategoryId)
                .Sum(x => x.Amount);

            if (spentAmount <= budget.LimitAmount)
            {
                continue;
            }

            var referenceId = $"budget:{budget.Id}:{budget.Year}:{budget.Month}:exceeded";
            var existing = await _notificationRepository.GetByReferenceAsync(userId, referenceId, NotificationType.BudgetExceeded, cancellationToken);
            if (existing is not null)
            {
                continue;
            }

            await _notificationRepository.AddAsync(new Notification
            {
                UserId = userId,
                Type = NotificationType.BudgetExceeded,
                Title = "Budget exceeded",
                Message = $"Your {budget.Category.Name} spending has exceeded the monthly budget by {(spentAmount - budget.LimitAmount):C}.",
                ReferenceId = referenceId
            }, cancellationToken);

            changed = true;
        }

        if (changed)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
