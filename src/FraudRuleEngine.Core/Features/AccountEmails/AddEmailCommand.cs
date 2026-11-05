using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.AccountEmails;

public sealed record AddEmailCommand(AccountEmail AccountEmail);

public sealed class AddEmailCommandHandler(IAccountEmailRepository accountEmailRepository)
{
    public async Task Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        await accountEmailRepository.AddAsync(request.AccountEmail, cancellationToken);
    }
}
