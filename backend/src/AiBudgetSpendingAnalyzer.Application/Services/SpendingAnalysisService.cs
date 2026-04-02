using AiBudgetSpendingAnalyzer.Application.Common.Exceptions;
using AiBudgetSpendingAnalyzer.Application.DTOs.AI;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class SpendingAnalysisService : ISpendingAnalysisService
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAiInsightsProvider _aiInsightsProvider;

    public SpendingAnalysisService(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository,
        IAiInsightsProvider aiInsightsProvider)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _aiInsightsProvider = aiInsightsProvider;
    }

    public async Task<AiAnalysisDto> AnalyzeAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User profile was not found.");

        var transactions = await _transactionRepository.GetByUserAsync(
            userId,
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow,
            cancellationToken);

        return await _aiInsightsProvider.GenerateAsync(user, transactions, cancellationToken);
    }
}
