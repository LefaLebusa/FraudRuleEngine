using FraudRuleEngine.Domain;
using Microsoft.Extensions.Logging;

namespace FraudRuleEngine.Core.Features.Rules;

public sealed record EvaluateTriggeredRulesQuery(Transaction Transaction, List<Transaction> History);

public sealed class EvaluateTriggeredRulesQueryHandler(IEnumerable<IFraudRule> fraudRules,
                                                       ILogger<EvaluateTriggeredRulesQueryHandler> logger)
{
    public List<string> Handle(EvaluateTriggeredRulesQuery request)
    {
        var triggered = new List<string>();

        foreach (var rule in fraudRules)
        {
            var fired = rule.Evaluate(request.Transaction, request.History);

            // Trace: per-rule detail. Enable only for the Rules category; it is very chatty.
            logger.LogTrace("Rule {RuleName} {Outcome} for transaction {TransactionId}",
                            rule.RuleName, fired ? "fired" : "passed", request.Transaction.Id);

            if (fired)
            {
                triggered.Add(rule.RuleName);
            }
        }

        logger.LogDebug("{TriggeredCount} rule(s) fired for transaction {TransactionId}",
                        triggered.Count, request.Transaction.Id);
        return triggered;
    }
}
/* This is how you explain this line ".Where(rule => rule.Evaluate(request.Transaction, request.History)) "
    var evaluationResults = new List<String>();

        foreach (var rule in fraudRules)

        {
            if (rule.Evaluate(request.Transaction, request.History))

            {
                evaluationResults.Add(rule.RuleName);
            }
        }
        return evaluationResults;
 */