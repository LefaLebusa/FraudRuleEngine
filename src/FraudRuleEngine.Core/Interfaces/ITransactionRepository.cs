using FraudRuleEngine.Core.Models;

namespace FraudRuleEngine.Core.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> AddAsync(Transaction transaction);
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId);
    Task<IEnumerable<Transaction>> GetRecentByAccountIdAsync(string accountId, TimeSpan window);
    Task<IEnumerable<Transaction>> GetFlaggedAsync();
    Task UpdateAsync(Transaction transaction);
}
