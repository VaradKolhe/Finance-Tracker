using AiBudgetSpendingAnalyzer.Application.DTOs.Reports;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Validators;

public class ReportRequestDtoValidator : AbstractValidator<ReportRequestDto>
{
    public ReportRequestDtoValidator()
    {
        RuleFor(x => x.Period)
            .NotEmpty()
            .Must(period => period.Equals("weekly", StringComparison.OrdinalIgnoreCase) || period.Equals("monthly", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Period must be weekly or monthly.");
    }
}
