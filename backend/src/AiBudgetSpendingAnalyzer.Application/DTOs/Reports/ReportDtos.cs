using AiBudgetSpendingAnalyzer.Application.DTOs.Budgets;
using AiBudgetSpendingAnalyzer.Application.DTOs.Transactions;

namespace AiBudgetSpendingAnalyzer.Application.DTOs.Reports;

public record ReportRequestDto(string Period, DateTime? FromUtc, DateTime? ToUtc);

public record ReportMetricDto(string Label, decimal Value, string Description);

public record ReportDto(
    string Period,
    DateTime FromUtc,
    DateTime ToUtc,
    IReadOnlyCollection<ReportMetricDto> Metrics,
    IReadOnlyCollection<BudgetProgressDto> Budgets,
    IReadOnlyCollection<TransactionDto> Transactions,
    IReadOnlyCollection<string> Recommendations);
