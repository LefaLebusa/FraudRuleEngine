using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;
using FraudRuleEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FraudRuleEngine.Infrastructure.Repositories;

public class TransactionRepository(FraudDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Transactions
            .Include(t => t.FraudEvaluation)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<List<Transaction>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken)
        => context.Transactions
            .Include(t => t.FraudEvaluation)
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);

    public Task<List<Transaction>> GetRecentByAccountIdAsync(string accountId, TimeSpan window, CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow - window;
        return context.Transactions
            .Where(t => t.AccountId == accountId && t.Timestamp >= since)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken)
        => context.Transactions
            .Include(t => t.FraudEvaluation)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);

    public Task<List<Transaction>> GetFlaggedTransactionsAsync(List<string> riskLevels, CancellationToken cancellationToken)
        => context.Transactions
            .Include(t => t.FraudEvaluation)
            .Where(t => t.FraudEvaluation != null
                        && t.FraudEvaluation.IsFlagged
                        && riskLevels.Contains(t.FraudEvaluation.RiskLevel))
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(cancellationToken);
}
