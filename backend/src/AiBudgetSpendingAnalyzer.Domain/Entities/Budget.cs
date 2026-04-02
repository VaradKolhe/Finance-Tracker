using AiBudgetSpendingAnalyzer.Domain.Common;

namespace AiBudgetSpendingAnalyzer.Domain.Entities;

public class Budget : BaseEntity
{
    public decimal LimitAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal AlertThresholdPercentage { get; set; } = 90m;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
