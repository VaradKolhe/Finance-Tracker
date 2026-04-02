using AiBudgetSpendingAnalyzer.Application.Common.Mappings;
using AiBudgetSpendingAnalyzer.Application.DTOs.Dashboard;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly INotificationRepository _notificationRepository;

    public DashboardService(
        ITransactionRepository transactionRepository,
        IBudgetRepository budgetRepository,
        INotificationRepository notificationRepository)
    {
        _transactionRepository = transactionRepository;
        _budgetRepository = budgetRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<DashboardDto> GetDashboardAsync(int userId, int? year = null, int? month = null, CancellationToken cancellationToken = default)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;
        var targetMonth = month ?? DateTime.UtcNow.Month;
        var periodStart = new DateTime(targetYear, targetMonth, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1);

        var periodTransactions = await _transactionRepository.GetByUserAsync(userId, periodStart, periodEnd, cancellationToken);
        var recentTransactions = await _transactionRepository.GetRecentAsync(userId, 5, cancellationToken);
        var budgets = await _budgetRepository.GetByUserAsync(userId, targetYear, targetMonth, cancellationToken);
        var notifications = await _notificationRepository.GetByUserAsync(userId, cancellationToken);

        var income = periodTransactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount);
        var expenses = periodTransactions.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount);
        var netSavings = income - expenses;
        var savingsRate = income <= 0 ? 0 : Math.Round((netSavings / income) * 100, 2);

        var categoryBreakdown = periodTransactions
            .Where(x => x.Type == TransactionType.Expense)
            .GroupBy(x => new { x.CategoryId, x.Category.Name, x.Category.ColorHex })
            .Select(group => new CategoryBreakdownDto(
                group.Key.CategoryId,
                group.Key.Name,
                group.Key.ColorHex,
                group.Sum(x => x.Amount),
                expenses == 0 ? 0 : Math.Round((group.Sum(x => x.Amount) / expenses) * 100, 2)))
            .OrderByDescending(x => x.Amount)
            .ToArray();

        var trendStart = periodStart.AddMonths(-5);
        var trendTransactions = await _transactionRepository.GetByUserAsync(userId, trendStart, periodEnd, cancellationToken);
        var monthlyTrend = Enumerable.Range(0, 6)
            .Select(offset => trendStart.AddMonths(offset))
            .Select(monthPoint =>
            {
                var monthTransactions = trendTransactions.Where(x => x.TransactionDateUtc.Year == monthPoint.Year && x.TransactionDateUtc.Month == monthPoint.Month);
                return new MonthlyTrendPointDto(
                    monthPoint.ToString("MMM yyyy"),
                    monthTransactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount),
                    monthTransactions.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount));
            })
            .ToArray();

        var budgetProgress = budgets
            .Select(budget => budget.ToBudgetProgressDto(periodTransactions
                .Where(x => x.Type == TransactionType.Expense && x.CategoryId == budget.CategoryId)
                .Sum(x => x.Amount)))
            .OrderByDescending(x => x.ProgressPercentage)
            .ToArray();

        return new DashboardDto(
            new DashboardSummaryDto(income, expenses, netSavings, savingsRate),
            categoryBreakdown,
            monthlyTrend,
            recentTransactions.Select(x => x.ToTransactionDto()).ToArray(),
            budgetProgress,
            notifications.OrderByDescending(x => x.CreatedAtUtc).Take(6).Select(x => x.ToNotificationDto()).ToArray());
    }
}
