using FluentValidation;

namespace FraudRuleEngine.Application.Orchestrators.Transactions;

public sealed class TransactionDtoValidator : AbstractValidator<TransactionDto>
{
    public TransactionDtoValidator()
    {
        RuleFor(x => x.AccountId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Amount)
            .InclusiveBetween(0.01m, 1_000_000_000m);

        RuleFor(x => x.Currency)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches("^[A-Z]{3}$").WithMessage("Currency must be a 3-letter ISO code.");

        RuleFor(x => x.MerchantName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.MerchantCategory)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches("^[A-Z]{2}$").WithMessage("Country must be a 2-letter ISO code.");

        RuleFor(x => x.TransactionType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(type => type is "DEBIT" or "CREDIT")
            .WithMessage("TransactionType must be DEBIT or CREDIT.");

        RuleFor(x => x.ReferenceNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(64);
    }
}
