using AiBudgetSpendingAnalyzer.Application.DTOs.Categories;
using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Application.DTOs.Admin;

public record AdminUserDto(
    int Id,
    string FullName,
    string Email,
    UserRole Role,
    decimal MonthlyIncome,
    string CurrencyCode,
    int TransactionCount,
    DateTime CreatedAtUtc);

public record SystemAnalyticsDto(
    int TotalUsers,
    int TotalCategories,
    int TotalTransactions,
    decimal TotalIncome,
    decimal TotalExpenses,
    int ActiveAlerts);

public record AdminDashboardDto(
    SystemAnalyticsDto Analytics,
    IReadOnlyCollection<AdminUserDto> Users,
    IReadOnlyCollection<CategoryDto> Categories);
