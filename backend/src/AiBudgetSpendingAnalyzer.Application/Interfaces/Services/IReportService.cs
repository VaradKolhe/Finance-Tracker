using AiBudgetSpendingAnalyzer.Application.DTOs.Reports;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface IReportService
{
    Task<ReportDto> GenerateAsync(int userId, ReportRequestDto request, CancellationToken cancellationToken = default);
}
