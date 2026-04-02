using AiBudgetSpendingAnalyzer.Application.DTOs.Categories;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).MaximumLength(50);
        RuleFor(x => x.ColorHex).NotEmpty().Matches("^#(?:[0-9a-fA-F]{3}){1,2}$");
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).MaximumLength(50);
        RuleFor(x => x.ColorHex).NotEmpty().Matches("^#(?:[0-9a-fA-F]{3}){1,2}$");
    }
}
