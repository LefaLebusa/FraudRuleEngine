namespace FraudRuleEngine.Core.Features.Alertting
{
    public interface IEmailService
    {
        public Task SendAlertAsync(string recipientEmail, string accountId, decimal amount, string riskLevel, List<string> triggeredRules, CancellationToken cancellationToken);
    }
}