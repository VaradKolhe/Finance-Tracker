using AiBudgetSpendingAnalyzer.Domain.Common;
using AiBudgetSpendingAnalyzer.Domain.Enums;

namespace AiBudgetSpendingAnalyzer.Domain.Entities;

public class Notification : BaseEntity
{
    public NotificationType Type { get; set; } = NotificationType.Info;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string ReferenceId { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
