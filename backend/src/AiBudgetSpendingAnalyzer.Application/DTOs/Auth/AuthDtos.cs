using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Application.DTOs.Auth;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    decimal MonthlyIncome,
    string FinancialGoal,
    string CurrencyCode,
    bool PrefersDarkMode);

public record LoginRequest(string Email, string Password);

public record AuthenticatedUserDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    decimal MonthlyIncome,
    string FinancialGoal,
    string CurrencyCode,
    bool PrefersDarkMode);

public record AuthResponseDto(string Token, DateTime ExpiresAtUtc, AuthenticatedUserDto User);
