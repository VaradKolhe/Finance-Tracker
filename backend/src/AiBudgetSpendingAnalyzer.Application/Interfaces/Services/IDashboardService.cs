using AiBudgetSpendingAnalyzer.Application.DTOs.Dashboard;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(int userId, int? year = null, int? month = null, CancellationToken cancellationToken = default);
}
