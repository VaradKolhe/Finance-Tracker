using AiBudgetSpendingAnalyzer.Domain.Common;
using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Domain.Entities;

public class Transaction : BaseEntity
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime TransactionDateUtc { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public RecurringFrequency? RecurringFrequency { get; set; }
    public DateTime? NextOccurrenceDateUtc { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
