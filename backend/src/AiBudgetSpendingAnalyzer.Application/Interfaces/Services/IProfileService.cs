using AiBudgetSpendingAnalyzer.Application.DTOs.Profile;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface IProfileService
{
    Task<UserProfileDto> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
}
