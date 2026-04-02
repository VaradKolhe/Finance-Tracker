using AiBudgetSpendingAnalyzer.Application.Common.Mappings;
using AiBudgetSpendingAnalyzer.Application.DTOs.Reports;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Domain.Enums;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class ReportService : IReportService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IValidator<ReportRequestDto> _validator;

    public ReportService(
        ITransactionRepository transactionRepository,
        IBudgetRepository budgetRepository,
        IValidator<ReportRequestDto> validator)
    {
        _transactionRepository = transactionRepository;
        _budgetRepository = budgetRepository;
        _validator = validator;
    }

    public async Task<ReportDto> GenerateAsync(int userId, ReportRequestDto request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var period = request.Period.Trim().ToLowerInvariant();
        var (fromUtc, toUtc) = ResolveDateRange(period, request.FromUtc, request.ToUtc);
        var transactions = await _transactionRepository.GetByUserAsync(userId, fromUtc, toUtc, cancellationToken);
        var budgets = await _budgetRepository.GetByUserAsync(userId, toUtc.Year, toUtc.Month, cancellationToken);

        var totalIncome = transactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount);
        var totalExpenses = transactions.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount);
        var netSavings = totalIncome - totalExpenses;
        var totalDays = Math.Max(1, (decimal)Math.Ceiling((toUtc - fromUtc).TotalDays));
        var averageDailySpend = Math.Round(totalExpenses / totalDays, 2);

        var metrics = new[]
        {
            new ReportMetricDto("Income", totalIncome, "Total income received in the selected period."),
            new ReportMetricDto("Expenses", totalExpenses, "Total expenses recorded in the selected period."),
            new ReportMetricDto("Net savings", netSavings, "Income minus expenses."),
            new ReportMetricDto("Average daily spend", averageDailySpend, "Average expense spend per day.")
        };

        var budgetProgress = budgets
            .Select(budget => budget.ToBudgetProgressDto(transactions
                .Where(x => x.Type == TransactionType.Expense && x.CategoryId == budget.CategoryId)
                .Sum(x => x.Amount)))
            .OrderByDescending(x => x.ProgressPercentage)
            .ToArray();

        return new ReportDto(
            period,
            fromUtc,
            toUtc,
            metrics,
            budgetProgress,
            transactions.OrderByDescending(x => x.TransactionDateUtc).Select(x => x.ToTransactionDto()).ToArray(),
            BuildRecommendations(totalIncome, totalExpenses, transactions));
    }

    private static (DateTime FromUtc, DateTime ToUtc) ResolveDateRange(string period, DateTime? fromUtc, DateTime? toUtc)
    {
        if (fromUtc.HasValue && toUtc.HasValue && fromUtc <= toUtc)
        {
            return (fromUtc.Value.ToUniversalTime(), toUtc.Value.ToUniversalTime());
        }

        var now = DateTime.UtcNow;
        if (period == "weekly")
        {
            return (now.Date.AddDays(-6), now);
        }

        return (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc), now);
    }

    private static string[] BuildRecommendations(decimal totalIncome, decimal totalExpenses, IReadOnlyCollection<Domain.Entities.Transaction> transactions)
    {
        var recommendations = new List<string>();

        if (totalExpenses > totalIncome)
        {
            recommendations.Add("Expenses are higher than income for this period. Review discretionary categories to rebalance spending.");
        }

        var topExpenseCategory = transactions
            .Where(x => x.Type == TransactionType.Expense)
            .GroupBy(x => x.Category.Name)
            .Select(group => new { Category = group.Key, Amount = group.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Amount)
            .FirstOrDefault();

        if (topExpenseCategory is not null && totalExpenses > 0)
        {
            var share = Math.Round((topExpenseCategory.Amount / totalExpenses) * 100, 2);
            recommendations.Add($"{topExpenseCategory.Category} accounts for {share}% of your expenses. This is a strong category to optimize first.");
        }

        if (!recommendations.Any())
        {
            recommendations.Add("Your spending is balanced in the selected period. Keep monitoring budgets to stay ahead of changes.");
        }

        return recommendations.ToArray();
    }
}
