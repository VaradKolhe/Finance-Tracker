using AiBudgetSpendingAnalyzer.Application.DTOs.Transactions;
using FluentValidation;

namespace AiBudgetSpendingAnalyzer.Application.Validators;

public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.TransactionDateUtc).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
{
    public UpdateTransactionRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.TransactionDateUtc).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
