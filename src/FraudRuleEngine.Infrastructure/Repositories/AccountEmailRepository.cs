using FraudRuleEngine.Core.Features.AccountEmails;
using FraudRuleEngine.Domain;
using FraudRuleEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FraudRuleEngine.Infrastructure.Repositories;

public class AccountEmailRepository(FraudDbContext context) : IAccountEmailRepository
{
    // One alert email per account: an existing entry is replaced.
    public async Task AddAsync(AccountEmail accountEmail, CancellationToken cancellationToken)
    {
        var existing = await context.AccountEmails.FindAsync([accountEmail.AccountId], cancellationToken);
        if (existing is null)
        {
            context.AccountEmails.Add(accountEmail);
        }
        else
        {
            context.Entry(existing).CurrentValues.SetValues(accountEmail);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<string?> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken)
        => context.AccountEmails
            .Where(a => a.AccountId == accountId)
            .Select(a => a.Email)
            .FirstOrDefaultAsync(cancellationToken);
}
