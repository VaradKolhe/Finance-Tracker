using AiBudgetSpendingAnalyzer.Domain.Common;
using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public decimal MonthlyIncome { get; set; }
    public string FinancialGoal { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "USD";
    public bool PrefersDarkMode { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
