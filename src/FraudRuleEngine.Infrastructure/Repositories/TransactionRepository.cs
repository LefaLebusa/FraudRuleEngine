using FraudRuleEngine.Core.Interfaces;
using FraudRuleEngine.Core.Models;
using FraudRuleEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FraudRuleEngine.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly FraudDbContext _context;

    public TransactionRepository(FraudDbContext context) => _context = context;

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public Task<Transaction?> GetByIdAsync(Guid id)
        => _context.Transactions
            .Include(t => t.FraudEvaluation)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId)
        => await _context.Transactions
            .Include(t => t.FraudEvaluation)
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();

    public async Task<IEnumerable<Transaction>> GetRecentByAccountIdAsync(string accountId, TimeSpan window)
    {
        var since = DateTime.UtcNow - window;
        return await _context.Transactions
            .Where(t => t.AccountId == accountId && t.Timestamp >= since)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetFlaggedAsync()
        => await _context.Transactions
            .Include(t => t.FraudEvaluation)
            .Where(t => t.FraudEvaluation != null && t.FraudEvaluation.IsFlagged)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();

    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }
}
