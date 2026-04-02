using AiBudgetSpendingAnalyzer.Application.DTOs.Budgets;
using AiBudgetSpendingAnalyzer.Application.DTOs.Notifications;
using AiBudgetSpendingAnalyzer.Application.DTOs.Transactions;

namespace AiBudgetSpendingAnalyzer.Application.DTOs.Dashboard;

public record DashboardSummaryDto(
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetSavings,
    decimal SavingsRatePercentage);

public record CategoryBreakdownDto(
    int CategoryId,
    string CategoryName,
    string CategoryColorHex,
    decimal Amount,
    decimal Percentage);

public record MonthlyTrendPointDto(
    string Label,
    decimal Income,
    decimal Expenses);

public record DashboardDto(
    DashboardSummaryDto Summary,
    IReadOnlyCollection<CategoryBreakdownDto> CategoryBreakdown,
    IReadOnlyCollection<MonthlyTrendPointDto> MonthlyTrend,
    IReadOnlyCollection<TransactionDto> RecentTransactions,
    IReadOnlyCollection<BudgetProgressDto> Budgets,
    IReadOnlyCollection<NotificationDto> Notifications);
