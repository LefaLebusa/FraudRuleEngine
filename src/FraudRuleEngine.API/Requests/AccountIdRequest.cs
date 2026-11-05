using FluentValidation;

namespace FraudRuleEngine.API.Requests;

// The accountId route value, validated the same way as the account id in a transaction
public sealed record AccountIdRequest(string AccountId);

public sealed class AccountIdRequestValidator : AbstractValidator<AccountIdRequest>
{
    public AccountIdRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);
    }
}
