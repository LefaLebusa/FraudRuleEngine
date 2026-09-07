using FraudRuleEngine.Core.Models;
using FraudRuleEngine.Core.Rules;

namespace FraudRuleEngine.Application.Rules;

public class HighAmountRule : IFraudRule
{
    private const decimal Threshold = 50_000m;

    public string RuleName => "HIGH_AMOUNT";

    public bool Evaluate(Transaction transaction, IEnumerable<Transaction> accountHistory)
        => transaction.Amount > Threshold;
}
