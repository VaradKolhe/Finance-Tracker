namespace AiBudgetSpendingAnalyzer.Application.DTOs.AI;

public record AiAnalysisDto(
    string Summary,
    IReadOnlyCollection<string> Insights,
    IReadOnlyCollection<string> Recommendations);
