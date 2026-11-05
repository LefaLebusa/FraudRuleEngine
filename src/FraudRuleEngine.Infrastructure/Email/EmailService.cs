using System.Net;
using System.Net.Mail;
using FraudRuleEngine.Core.Features.Alertting;
using FraudRuleEngine.Domain;
using Microsoft.Extensions.Logging;

namespace FraudRuleEngine.Infrastructure.Email;

public sealed class EmailService(EmailSettings settings, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendAlertAsync(string recipientEmail, string accountId, decimal amount, string riskLevel, List<string> triggeredRules, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort)
            {
                Credentials = new NetworkCredential(settings.SenderEmail, settings.SenderPassword),
                EnableSsl = true
            };

            var subject = $"🚨 FRAUD ALERT - {riskLevel} Risk Transaction";
            var body = GenerateEmailBody(accountId, amount, riskLevel, triggeredRules);

            using var mailMessage = new MailMessage(settings.SenderEmail, recipientEmail, subject, body)
            {
                IsBodyHtml = true
            };

            logger.LogDebug("Sending {RiskLevel} fraud alert for account {AccountId}", riskLevel, accountId);
            await client.SendMailAsync(mailMessage, cancellationToken);
            // The recipient address is deliberately not logged
            logger.LogInformation("Fraud alert ({RiskLevel}) sent for account {AccountId}", riskLevel, accountId);
        }
        catch (Exception ex) when (ex is SmtpException or InvalidOperationException or FormatException)
        {
            // A failed alert must not fail the transaction submission
            logger.LogError(ex, "Failed to send fraud alert for account {AccountId}", accountId);
        }
    }

    private static string GenerateEmailBody(string accountId, decimal amount, string riskLevel, List<string> triggeredRules)
    {
        var riskColor = riskLevel switch
        {
            RiskLevels.High => "#d32f2f",
            RiskLevels.Medium => "#f57c00",
            _ => "#388e3c"
        };

        var safeRiskLevel = WebUtility.HtmlEncode(riskLevel);
        var safeAccountId = WebUtility.HtmlEncode(accountId);
        var safeRules = string.Join("", triggeredRules.Select(r => $"<li>{WebUtility.HtmlEncode(r)}</li>"));

        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='background-color: {riskColor}; color: white; padding: 20px; border-radius: 5px;'>
                    <h2>⚠️ Fraud Detection Alert</h2>
                </div>

                <div style='padding: 20px; border: 1px solid #ddd; margin-top: 10px;'>
                    <p><strong>Risk Level:</strong> <span style='color: {riskColor}; font-weight: bold;'>{safeRiskLevel}</span></p>
                    <p><strong>Account ID:</strong> {safeAccountId}</p>
                    <p><strong>Transaction Amount:</strong> R {amount:N2}</p>
                    <p><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>

                    <h3>Triggered Rules:</h3>
                    <ul>
                        {safeRules}
                    </ul>

                    <p style='color: #666; font-size: 12px; margin-top: 20px;'>
                        This is an automated alert from the Fraud Rule Engine.
                        Please review this transaction immediately if you did not authorize it.
                    </p>
                </div>
            </body>
            </html>
        ";
    }
}
