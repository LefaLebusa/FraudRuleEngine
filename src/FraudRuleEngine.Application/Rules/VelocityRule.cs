using FraudRuleEngine.Core.Models;
using FraudRuleEngine.Core.Rules;

namespace FraudRuleEngine.Application.Rules;

// Flags when account has more than 5 transactions within a 10-minute window
public class VelocityRule : IFraudRule
{
    private const int MaxTransactions = 5;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(10);

    public string RuleName => "HIGH_VELOCITY";

    public bool Evaluate(Transaction transaction, IEnumerable<Transaction> accountHistory)
    {
        var windowStart = transaction.Timestamp - Window;
        var count = accountHistory.Count(t =>
            t.Id != transaction.Id &&
            t.Timestamp >= windowStart &&
            t.Timestamp <= transaction.Timestamp);

        return count >= MaxTransactions;
    }
}
