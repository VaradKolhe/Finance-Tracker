using AiBudgetSpendingAnalyzer.Domain.Common;

namespace AiBudgetSpendingAnalyzer.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#3B82F6";
    public bool IsSystemDefined { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
