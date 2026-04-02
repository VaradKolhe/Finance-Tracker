using AiBudgetSpendingAnalyzer.Application.Common.Mappings;
using AiBudgetSpendingAnalyzer.Application.DTOs.Admin;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly INotificationRepository _notificationRepository;

    public AdminService(
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        INotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken: cancellationToken);
        var notifications = await _notificationRepository.GetAllAsync(cancellationToken);

        var countsByUser = transactions
            .GroupBy(x => x.UserId)
            .ToDictionary(group => group.Key, group => group.Count());

        var adminUsers = users
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(user => new AdminUserDto(
                user.Id,
                $"{user.FirstName} {user.LastName}".Trim(),
                user.Email,
                user.Role,
                user.MonthlyIncome,
                user.CurrencyCode,
                countsByUser.GetValueOrDefault(user.Id, 0),
                user.CreatedAtUtc))
            .ToArray();

        var analytics = new SystemAnalyticsDto(
            users.Count,
            categories.Count,
            transactions.Count,
            transactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount),
            transactions.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount),
            notifications.Count(x => !x.IsRead));

        return new AdminDashboardDto(
            analytics,
            adminUsers,
            categories.Select(x => x.ToCategoryDto()).ToArray());
    }
}
