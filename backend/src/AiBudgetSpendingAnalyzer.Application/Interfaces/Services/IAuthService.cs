using AiBudgetSpendingAnalyzer.Application.DTOs.Auth;

namespace AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync();
}
