using AiBudgetSpendingAnalyzer.Application.DTOs.Budgets;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Validators;

public class CreateBudgetRequestValidator : AbstractValidator<CreateBudgetRequest>
{
    public CreateBudgetRequestValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.LimitAmount).GreaterThan(0);
        RuleFor(x => x.AlertThresholdPercentage).InclusiveBetween(1, 100);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
    }
}

public class UpdateBudgetRequestValidator : AbstractValidator<UpdateBudgetRequest>
{
    public UpdateBudgetRequestValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.LimitAmount).GreaterThan(0);
        RuleFor(x => x.AlertThresholdPercentage).InclusiveBetween(1, 100);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
    }
}
