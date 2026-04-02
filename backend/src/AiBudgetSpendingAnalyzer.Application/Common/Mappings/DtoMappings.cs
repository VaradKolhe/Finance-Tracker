using AiBudgetSpendingAnalyzer.Application.DTOs.Auth;
using AiBudgetSpendingAnalyzer.Application.DTOs.Budgets;
using AiBudgetSpendingAnalyzer.Application.DTOs.Categories;
using AiBudgetSpendingAnalyzer.Application.DTOs.Notifications;
using AiBudgetSpendingAnalyzer.Application.DTOs.Profile;
using AiBudgetSpendingAnalyzer.Application.DTOs.Transactions;
using AiBudgetSpendingAnalyzer.Domain.Entities;

namespace AiBudgetSpendingAnalyzer.Application.Common.Mappings;

public static class DtoMappings
{
    public static AuthenticatedUserDto ToAuthenticatedUserDto(this User user) =>
        new(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            user.MonthlyIncome,
            user.FinancialGoal,
            user.CurrencyCode,
            user.PrefersDarkMode);

    public static UserProfileDto ToUserProfileDto(this User user) =>
        new(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            user.MonthlyIncome,
            user.FinancialGoal,
            user.CurrencyCode,
            user.PrefersDarkMode);

    public static CategoryDto ToCategoryDto(this Category category) =>
        new(
            category.Id,
            category.Name,
            category.Icon,
            category.ColorHex,
            category.IsSystemDefined,
            category.UserId);

    public static TransactionDto ToTransactionDto(this Transaction transaction) =>
        new(
            transaction.Id,
            transaction.Amount,
            transaction.Type,
            transaction.CategoryId,
            transaction.Category.Name,
            transaction.Category.ColorHex,
            transaction.Category.Icon,
            transaction.TransactionDateUtc,
            transaction.Notes,
            transaction.IsRecurring,
            transaction.RecurringFrequency,
            transaction.NextOccurrenceDateUtc);

    public static NotificationDto ToNotificationDto(this Notification notification) =>
        new(
            notification.Id,
            notification.Type,
            notification.Title,
            notification.Message,
            notification.IsRead,
            notification.CreatedAtUtc);

    public static BudgetProgressDto ToBudgetProgressDto(this Budget budget, decimal spentAmount)
    {
        var remaining = budget.LimitAmount - spentAmount;
        var progress = budget.LimitAmount == 0
            ? 0
            : Math.Round((spentAmount / budget.LimitAmount) * 100, 2);

        return new BudgetProgressDto(
            budget.Id,
            budget.CategoryId,
            budget.Category.Name,
            budget.Category.ColorHex,
            budget.LimitAmount,
            spentAmount,
            remaining,
            progress,
            spentAmount > budget.LimitAmount,
            budget.Month,
            budget.Year);
    }
}
