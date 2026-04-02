namespace AiBudgetSpendingAnalyzer.Application.DTOs.Budgets;

public record BudgetDto(
    int Id,
    int CategoryId,
    string CategoryName,
    string CategoryColorHex,
    decimal LimitAmount,
    decimal AlertThresholdPercentage,
    int Month,
    int Year);

public record BudgetProgressDto(
    int BudgetId,
    int CategoryId,
    string CategoryName,
    string CategoryColorHex,
    decimal LimitAmount,
    decimal SpentAmount,
    decimal RemainingAmount,
    decimal ProgressPercentage,
    bool IsExceeded,
    int Month,
    int Year);

public record CreateBudgetRequest(
    int CategoryId,
    decimal LimitAmount,
    decimal AlertThresholdPercentage,
    int Month,
    int Year);

public record UpdateBudgetRequest(
    int CategoryId,
    decimal LimitAmount,
    decimal AlertThresholdPercentage,
    int Month,
    int Year);
