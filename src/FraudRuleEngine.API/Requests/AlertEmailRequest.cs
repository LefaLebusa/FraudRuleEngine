using FluentValidation;

namespace FraudRuleEngine.API.Requests;

// Body of PUT api/accounts/{accountId}/alert-email
public sealed record AlertEmailRequest(string Email);

public sealed class AlertEmailRequestValidator : AbstractValidator<AlertEmailRequest>
{
    public AlertEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);
    }
}
