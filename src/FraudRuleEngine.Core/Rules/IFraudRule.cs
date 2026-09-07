using FraudRuleEngine.Core.Models;

namespace FraudRuleEngine.Core.Rules;

public interface IFraudRule
{
    string RuleName { get; }
    bool Evaluate(Transaction transaction, IEnumerable<Transaction> accountHistory);
}
