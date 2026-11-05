using FraudRuleEngine.Core.Features.Alertting;
using FraudRuleEngine.Core.Features.FraudEvaluations;
using FraudRuleEngine.Core.Features.Rules;
using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;
using Microsoft.Extensions.Logging;

namespace FraudRuleEngine.Application.Orchestrators.Evaluations;

public sealed record EvaluationOrchestrator(Guid TransactionId);

public sealed class EvaluationOrchestratorHandler(GetByIdQueryHandler getByIdQueryHandler,
                                                  GetRecentByAccountQueryHandler recentTransactionsQueryHandler,
                                                  CreateFraudEvaluationCommandHandler createFraudEvaluationCommandHandler,
                                                  EvaluateTriggeredRulesQueryHandler rulesEvaluationQueryHandler,
                                                  SendAlertNotificationHandler sendAlertNotificationHandler,
                                                  ILogger<EvaluationOrchestratorHandler> logger)
{
    private readonly TimeSpan _evaluationWindow = TimeSpan.FromHours(24);

    public async Task<FraudEvaluation> Handle(EvaluationOrchestrator request, CancellationToken cancellationToken)
    {
        // 1. Retrieve the transaction
        var transaction = await getByIdQueryHandler.Handle(new GetByIdQuery(request.TransactionId), cancellationToken)
            ?? throw new InvalidOperationException($"Transaction with ID {request.TransactionId} not found.");

        // 2. Retrieve the account history for the evaluation window
        var history = await recentTransactionsQueryHandler.Handle(new GetRecentByAccountQuery(transaction.AccountId, _evaluationWindow), cancellationToken);
        logger.LogDebug("Evaluating transaction {TransactionId} against {HistoryCount} transaction(s) in the last {WindowHours}h",
                        transaction.Id, history.Count, _evaluationWindow.TotalHours);

        // 3. Evaluate the fraud rules
        var triggeredRules = rulesEvaluationQueryHandler.Handle(new EvaluateTriggeredRulesQuery(transaction, history));

        // 4. Store the evaluation
        var evaluation = FraudEvaluation.Create(transaction.Id, triggeredRules);
        await createFraudEvaluationCommandHandler.Handle(new CreateFraudEvaluationCommand(evaluation), cancellationToken);

        var rules = triggeredRules.Count == 0 ? "none" : string.Join(", ", triggeredRules);
        logger.LogInformation("Transaction {TransactionId} evaluated: risk {RiskLevel}, rules {TriggeredRules}",
                              transaction.Id, evaluation.RiskLevel, rules);

        // 5. Alert on high-risk transactions
        if (ShouldSendAlert(evaluation))
        {
            logger.LogWarning("High-risk transaction {TransactionId} on account {AccountId}; sending alert",
                              transaction.Id, transaction.AccountId);
            await sendAlertNotificationHandler.Handle(new SendAlertNotification(transaction, evaluation), cancellationToken);
        }

        return evaluation;
    }

    private static bool ShouldSendAlert(FraudEvaluation evaluation)
        => evaluation.IsFlagged && evaluation.RiskLevel == RiskLevels.High;
}
