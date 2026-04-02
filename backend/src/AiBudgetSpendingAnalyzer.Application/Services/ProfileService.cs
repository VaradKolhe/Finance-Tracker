using AiBudgetSpendingAnalyzer.Application.Common.Exceptions;
using AiBudgetSpendingAnalyzer.Application.Common.Mappings;
using AiBudgetSpendingAnalyzer.Application.DTOs.Profile;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Repositories;
using AiBudgetSpendingAnalyzer.Application.Interfaces.Services;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProfileRequest> _validator;

    public ProfileService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateProfileRequest> validator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User profile was not found.");

        return user.ToUserProfileDto();
    }

    public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User profile was not found.");

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.MonthlyIncome = request.MonthlyIncome;
        user.FinancialGoal = request.FinancialGoal.Trim();
        user.CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant();
        user.PrefersDarkMode = request.PrefersDarkMode;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToUserProfileDto();
    }
}
