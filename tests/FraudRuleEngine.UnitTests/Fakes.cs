using FraudRuleEngine.Core.Features.AccountEmails;
using FraudRuleEngine.Core.Features.Alertting;
using FraudRuleEngine.Core.Features.FraudEvaluations;
using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;

namespace FraudRuleEngine.UnitTests;

internal sealed class FakeTransactionRepository : ITransactionRepository
{
    public List<Transaction> Items { get; } = [];

    public Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        Items.Add(transaction);
        return Task.CompletedTask;
    }

    public Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(Items.FirstOrDefault(t => t.Id == id));

    public Task<List<Transaction>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken)
        => Task.FromResult(Items.Where(t => t.AccountId == accountId).ToList());

    public Task<List<Transaction>> GetRecentByAccountIdAsync(string accountId, TimeSpan window, CancellationToken cancellationToken)
        => Task.FromResult(Items.Where(t => t.AccountId == accountId).ToList());

    public Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult(Items.ToList());

    public Task<List<Transaction>> GetFlaggedTransactionsAsync(List<string> riskLevels, CancellationToken cancellationToken)
        => Task.FromResult(Items.Where(t => t.FraudEvaluation is { IsFlagged: true } e && riskLevels.Contains(e.RiskLevel)).ToList());
}

internal sealed class FakeFraudEvaluationRepository : IFraudEvaluationRepository
{
    public List<FraudEvaluation> Saved { get; } = [];

    public Task AddEvaluationAsync(FraudEvaluation evaluation, CancellationToken cancellationToken)
    {
        Saved.Add(evaluation);
        return Task.CompletedTask;
    }
}

internal sealed class FakeAccountEmailRepository : IAccountEmailRepository
{
    public Dictionary<string, string> Emails { get; } = [];

    public Task AddAsync(AccountEmail accountEmail, CancellationToken cancellationToken)
    {
        Emails[accountEmail.AccountId] = accountEmail.Email;
        return Task.CompletedTask;
    }

    public Task<string?> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken)
        => Task.FromResult(Emails.GetValueOrDefault(accountId));
}

internal sealed class FakeEmailService : IEmailService
{
    public List<(string RecipientEmail, string RiskLevel)> Sent { get; } = [];

    public Task SendAlertAsync(string recipientEmail, string accountId, decimal amount, string riskLevel, List<string> triggeredRules, CancellationToken cancellationToken)
    {
        Sent.Add((recipientEmail, riskLevel));
        return Task.CompletedTask;
    }
}
