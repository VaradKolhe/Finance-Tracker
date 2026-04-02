using AiBudgetSpendingAnalyzer.Application.DTOs.Admin;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}
