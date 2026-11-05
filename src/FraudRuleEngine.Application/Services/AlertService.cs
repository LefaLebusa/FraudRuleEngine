using FraudRuleEngine.Core.Models;

namespace FraudRuleEngine.Application.Services;

public class AlertService
{
    private readonly EmailService _emailService;
    private readonly Dictionary<string, string> _accountEmails;

    public AlertService(EmailService emailService)
    {
        _emailService = emailService;
        _accountEmails = new();
    }

    public void RegisterAccount(string accountId, string email)
    {
        _accountEmails[accountId] = email;
    }

    public async Task SendAlertIfNeededAsync(Transaction transaction, FraudEvaluation evaluation)
    {
        if (!evaluation.IsFlagged || evaluation.RiskLevel != "HIGH")
            return;

        if (!_accountEmails.TryGetValue(transaction.AccountId, out var email))
            return;

        await _emailService.SendAlertAsync(
            email,
            transaction.AccountId,
            transaction.Amount,
            evaluation.RiskLevel,
            evaluation.TriggeredRules
        );
    }

    public void SetupTestAccount()
    {
        RegisterAccount("TEST_ACCOUNT", "test@example.com");
    }
}
