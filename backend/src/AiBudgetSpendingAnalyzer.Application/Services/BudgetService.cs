using AiBudgetSpendingAnalyzer.Application.Common.Exceptions;
using AiBudgetSpendingAnalyzer.Application.Common.Mappings;
using AiBudgetSpendingAnalyzer.Application.DTOs.Budgets;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using AiBudgetSpendingAnalyzer.Domain.Enums;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateBudgetRequest> _createValidator;
    private readonly IValidator<UpdateBudgetRequest> _updateValidator;

    public BudgetService(
        IBudgetRepository budgetRepository,
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateBudgetRequest> createValidator,
        IValidator<UpdateBudgetRequest> updateValidator)
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyCollection<BudgetProgressDto>> GetBudgetsAsync(int userId, int? year = null, int? month = null, CancellationToken cancellationToken = default)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;
        var targetMonth = month ?? DateTime.UtcNow.Month;
        var budgets = await _budgetRepository.GetByUserAsync(userId, targetYear, targetMonth, cancellationToken);
        var expenses = await GetExpensesForMonthAsync(userId, targetYear, targetMonth, cancellationToken);

        return budgets
            .OrderBy(x => x.Category.Name)
            .Select(budget => budget.ToBudgetProgressDto(GetSpentAmount(expenses, budget.CategoryId)))
            .ToArray();
    }

    public async Task<BudgetProgressDto> CreateBudgetAsync(int userId, bool isAdmin, CreateBudgetRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var category = await GetAccessibleCategoryAsync(userId, isAdmin, request.CategoryId, cancellationToken);
        var existingBudget = await _budgetRepository.GetByCategoryAndPeriodAsync(userId, request.CategoryId, request.Year, request.Month, cancellationToken);

        if (existingBudget is not null)
        {
            throw new ConflictException("A budget already exists for this category and month.");
        }

        var budget = new Budget
        {
            UserId = userId,
            CategoryId = category.Id,
            Category = category,
            LimitAmount = request.LimitAmount,
            AlertThresholdPercentage = request.AlertThresholdPercentage,
            Month = request.Month,
            Year = request.Year
        };

        await _budgetRepository.AddAsync(budget, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildProgressAsync(userId, budget, cancellationToken);
    }

    public async Task<BudgetProgressDto> UpdateBudgetAsync(int userId, bool isAdmin, int budgetId, UpdateBudgetRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var budget = await _budgetRepository.GetByIdAsync(budgetId, cancellationToken)
            ?? throw new NotFoundException("Budget was not found.");

        if (budget.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to modify this budget.");
        }

        var category = await GetAccessibleCategoryAsync(userId, isAdmin, request.CategoryId, cancellationToken);
        var duplicate = await _budgetRepository.GetByCategoryAndPeriodAsync(userId, request.CategoryId, request.Year, request.Month, cancellationToken);
        if (duplicate is not null && duplicate.Id != budget.Id)
        {
            throw new ConflictException("Another budget already exists for this category and month.");
        }

        budget.CategoryId = category.Id;
        budget.Category = category;
        budget.LimitAmount = request.LimitAmount;
        budget.AlertThresholdPercentage = request.AlertThresholdPercentage;
        budget.Month = request.Month;
        budget.Year = request.Year;
        budget.UpdatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildProgressAsync(userId, budget, cancellationToken);
    }

    public async Task DeleteBudgetAsync(int userId, int budgetId, CancellationToken cancellationToken = default)
    {
        var budget = await _budgetRepository.GetByIdAsync(budgetId, cancellationToken)
            ?? throw new NotFoundException("Budget was not found.");

        if (budget.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to delete this budget.");
        }

        _budgetRepository.Remove(budget);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Category> GetAccessibleCategoryAsync(int userId, bool isAdmin, int categoryId, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        if (!category.IsSystemDefined && category.UserId != userId && !isAdmin)
        {
            throw new ForbiddenException("You do not have access to the selected category.");
        }

        return category;
    }

    private async Task<BudgetProgressDto> BuildProgressAsync(int userId, Budget budget, CancellationToken cancellationToken)
    {
        var expenses = await GetExpensesForMonthAsync(userId, budget.Year, budget.Month, cancellationToken);
        return budget.ToBudgetProgressDto(GetSpentAmount(expenses, budget.CategoryId));
    }

    private async Task<IReadOnlyCollection<Transaction>> GetExpensesForMonthAsync(int userId, int year, int month, CancellationToken cancellationToken)
    {
        var periodStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1);

        return await _transactionRepository.GetByUserAsync(userId, periodStart, periodEnd, cancellationToken);
    }

    private static decimal GetSpentAmount(IEnumerable<Transaction> expenses, int categoryId) =>
        expenses
            .Where(x => x.Type == TransactionType.Expense && x.CategoryId == categoryId)
            .Sum(x => x.Amount);
}
