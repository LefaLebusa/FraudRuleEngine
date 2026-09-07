using FraudRuleEngine.Core.Models;
using FraudRuleEngine.Core.Rules;

namespace FraudRuleEngine.Application.Rules;

// Flags suspiciously round amounts above R5 000 (common in structuring / test transactions)
public class RoundAmountRule : IFraudRule
{
    private const decimal MinAmount = 5_000m;

    public string RuleName => "ROUND_AMOUNT";

    public bool Evaluate(Transaction transaction, IEnumerable<Transaction> accountHistory)
        => transaction.Amount >= MinAmount && transaction.Amount % 1000 == 0;
}
