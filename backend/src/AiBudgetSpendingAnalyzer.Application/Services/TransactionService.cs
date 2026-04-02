using AiBudgetSpendingAnalyzer.Application.Common.Exceptions;
using AiBudgetSpendingAnalyzer.Application.Common.Mappings;
using AiBudgetSpendingAnalyzer.Application.DTOs.Transactions;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using AiBudgetSpendingAnalyzer.Domain.Entities;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateTransactionRequest> _createValidator;
    private readonly IValidator<UpdateTransactionRequest> _updateValidator;

    public TransactionService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        IValidator<CreateTransactionRequest> createValidator,
        IValidator<UpdateTransactionRequest> updateValidator)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyCollection<TransactionDto>> GetTransactionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionRepository.GetByUserAsync(userId, cancellationToken: cancellationToken);

        return transactions
            .OrderByDescending(x => x.TransactionDateUtc)
            .Select(x => x.ToTransactionDto())
            .ToArray();
    }

    public async Task<TransactionDto> CreateTransactionAsync(int userId, bool isAdmin, CreateTransactionRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var category = await GetAccessibleCategoryAsync(userId, isAdmin, request.CategoryId, cancellationToken);

        var transaction = new Transaction
        {
            UserId = userId,
            CategoryId = category.Id,
            Category = category,
            Amount = request.Amount,
            Type = request.Type,
            TransactionDateUtc = request.TransactionDateUtc.ToUniversalTime(),
            Notes = request.Notes.Trim(),
            IsRecurring = request.IsRecurring,
            RecurringFrequency = request.RecurringFrequency,
            NextOccurrenceDateUtc = request.NextOccurrenceDateUtc?.ToUniversalTime()
        };

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.EvaluateNotificationsAsync(userId, transaction.Id, cancellationToken);

        return transaction.ToTransactionDto();
    }

    public async Task<TransactionDto> UpdateTransactionAsync(int userId, bool isAdmin, int transactionId, UpdateTransactionRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var transaction = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken)
            ?? throw new NotFoundException("Transaction was not found.");

        if (transaction.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to modify this transaction.");
        }

        var category = await GetAccessibleCategoryAsync(userId, isAdmin, request.CategoryId, cancellationToken);

        transaction.CategoryId = category.Id;
        transaction.Category = category;
        transaction.Amount = request.Amount;
        transaction.Type = request.Type;
        transaction.TransactionDateUtc = request.TransactionDateUtc.ToUniversalTime();
        transaction.Notes = request.Notes.Trim();
        transaction.IsRecurring = request.IsRecurring;
        transaction.RecurringFrequency = request.RecurringFrequency;
        transaction.NextOccurrenceDateUtc = request.NextOccurrenceDateUtc?.ToUniversalTime();
        transaction.UpdatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.EvaluateNotificationsAsync(userId, transaction.Id, cancellationToken);

        return transaction.ToTransactionDto();
    }

    public async Task DeleteTransactionAsync(int userId, int transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken)
            ?? throw new NotFoundException("Transaction was not found.");

        if (transaction.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to delete this transaction.");
        }

        _transactionRepository.Remove(transaction);
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
}
