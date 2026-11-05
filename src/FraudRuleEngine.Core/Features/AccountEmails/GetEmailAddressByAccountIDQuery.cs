namespace FraudRuleEngine.Core.Features.AccountEmails;

public sealed record GetEmailAddressByAccountIDQuery(string AccountId);

public sealed class GetEmailAddressByAccountIDQueryHandler(IAccountEmailRepository accountEmailRepository)
{
    public async Task<string?> Handle(GetEmailAddressByAccountIDQuery request, CancellationToken cancellationToken)
    {
        return await accountEmailRepository.GetByAccountIdAsync(request.AccountId, cancellationToken);
    }
}
