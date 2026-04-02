using AiBudgetSpendingAnalyzer.Application.DTOs.AI;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface ISpendingAnalysisService
{
    Task<AiAnalysisDto> AnalyzeAsync(int userId, CancellationToken cancellationToken = default);
}
