using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Application.DTOs.Transactions;

public record TransactionDto(
    int Id,
    decimal Amount,
    TransactionType Type,
    int CategoryId,
    string CategoryName,
    string CategoryColorHex,
    string CategoryIcon,
    DateTime TransactionDateUtc,
    string Notes,
    bool IsRecurring,
    RecurringFrequency? RecurringFrequency,
    DateTime? NextOccurrenceDateUtc);

public record CreateTransactionRequest(
    decimal Amount,
    TransactionType Type,
    int CategoryId,
    DateTime TransactionDateUtc,
    string Notes,
    bool IsRecurring,
    RecurringFrequency? RecurringFrequency,
    DateTime? NextOccurrenceDateUtc);

public record UpdateTransactionRequest(
    decimal Amount,
    TransactionType Type,
    int CategoryId,
    DateTime TransactionDateUtc,
    string Notes,
    bool IsRecurring,
    RecurringFrequency? RecurringFrequency,
    DateTime? NextOccurrenceDateUtc);
