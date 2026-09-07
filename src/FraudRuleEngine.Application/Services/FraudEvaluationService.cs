using FraudRuleEngine.Core.Interfaces;
using FraudRuleEngine.Core.Models;
using FraudRuleEngine.Core.Rules;

namespace FraudRuleEngine.Application.Services;

public class FraudEvaluationService
{
    private readonly IEnumerable<IFraudRule> _rules;
    private readonly ITransactionRepository _repository;

    public FraudEvaluationService(IEnumerable<IFraudRule> rules, ITransactionRepository repository)
    {
        _rules = rules;
        _repository = repository;
    }

    public async Task<FraudEvaluation> EvaluateAsync(Transaction transaction)
    {
        var history = await _repository.GetRecentByAccountIdAsync(
            transaction.AccountId,
            TimeSpan.FromHours(24));

        var triggeredRules = _rules
            .Where(rule => rule.Evaluate(transaction, history))
            .Select(rule => rule.RuleName)
            .ToList();

        var evaluation = new FraudEvaluation
        {
            TransactionId = transaction.Id,
            IsFlagged = triggeredRules.Count > 0,
            TriggeredRules = triggeredRules,
            RiskLevel = triggeredRules.Count switch
            {
                0 => "LOW",
                1 => "MEDIUM",
                _ => "HIGH"
            }
        };

        await _repository.AddEvaluationAsync(evaluation);

        return evaluation;
    }
}
