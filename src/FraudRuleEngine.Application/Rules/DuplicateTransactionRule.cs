using FraudRuleEngine.Core.Models;
using FraudRuleEngine.Core.Rules;

namespace FraudRuleEngine.Application.Rules;

// Flags when the same amount appears more than once within 2 minutes for the same account
public class DuplicateTransactionRule : IFraudRule
{
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(2);

    public string RuleName => "DUPLICATE_TRANSACTION";

    public bool Evaluate(Transaction transaction, IEnumerable<Transaction> accountHistory)
    {
        var windowStart = transaction.Timestamp - Window;
        return accountHistory.Any(t =>
            t.Id != transaction.Id &&
            t.Amount == transaction.Amount &&
            t.MerchantName == transaction.MerchantName &&
            t.Timestamp >= windowStart &&
            t.Timestamp <= transaction.Timestamp);
    }
}
