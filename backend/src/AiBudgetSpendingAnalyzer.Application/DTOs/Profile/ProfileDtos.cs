using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Application.DTOs.Profile;

public record UserProfileDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    decimal MonthlyIncome,
    string FinancialGoal,
    string CurrencyCode,
    bool PrefersDarkMode);

public record UpdateProfileRequest(
    string FirstName,
    string LastName,
    decimal MonthlyIncome,
    string FinancialGoal,
    string CurrencyCode,
    bool PrefersDarkMode);
