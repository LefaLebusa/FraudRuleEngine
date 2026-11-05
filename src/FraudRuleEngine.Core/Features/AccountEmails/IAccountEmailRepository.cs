using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.AccountEmails;

public interface IAccountEmailRepository
{
    Task AddAsync(AccountEmail accountEmail, CancellationToken cancellationToken);
    Task<string?> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken);
}
