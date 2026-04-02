using AiBudgetSpendingAnalyzer.Application.DTOs.AI;
using AiBudgetSpendingAnalyzer.Domain.Entities;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface IAiInsightsProvider
{
    Task<AiAnalysisDto> GenerateAsync(User user, IReadOnlyCollection<Transaction> transactions, CancellationToken cancellationToken = default);
}
