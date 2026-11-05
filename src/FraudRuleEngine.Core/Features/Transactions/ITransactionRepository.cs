using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.Transactions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken);
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Transaction>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken);
    Task<List<Transaction>> GetRecentByAccountIdAsync(string accountId, TimeSpan window, CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Transaction>> GetFlaggedTransactionsAsync(List<string> riskLevels, CancellationToken cancellationToken);
}
