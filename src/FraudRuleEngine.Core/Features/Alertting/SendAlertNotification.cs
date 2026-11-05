using FraudRuleEngine.Core.Features.AccountEmails;
using FraudRuleEngine.Domain;
using Microsoft.Extensions.Logging;

namespace FraudRuleEngine.Core.Features.Alertting;

public sealed record SendAlertNotification(Transaction Transaction, FraudEvaluation FraudEvaluation);

public sealed class SendAlertNotificationHandler(IEmailService emailService,
                                                 GetEmailAddressByAccountIDQueryHandler getEmail,
                                                 ILogger<SendAlertNotificationHandler> logger)
{
    public async Task Handle(SendAlertNotification request, CancellationToken cancellationToken)
    {
        var email = await getEmail.Handle(new GetEmailAddressByAccountIDQuery(request.Transaction.AccountId), cancellationToken);
        if (email is null)
        {
            logger.LogWarning("No alert email registered for account {AccountId}; alert for transaction {TransactionId} not sent.",
                               request.Transaction.AccountId, request.Transaction.Id);
            return;
        }

        await emailService.SendAlertAsync(email,
                                          request.Transaction.AccountId,
                                          request.Transaction.Amount,
                                          request.FraudEvaluation.RiskLevel,
                                          request.FraudEvaluation.TriggeredRules,
                                          cancellationToken);
    }
}
