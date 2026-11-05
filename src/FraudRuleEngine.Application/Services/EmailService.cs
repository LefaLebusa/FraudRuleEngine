using System.Net;
using System.Net.Mail;

namespace FraudRuleEngine.Application.Services;

public class EmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword;

    public EmailService(string smtpServer, int smtpPort, string senderEmail, string senderPassword)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _senderEmail = senderEmail;
        _senderPassword = senderPassword;
    }

    public async Task SendAlertAsync(string recipientEmail, string accountId, decimal amount, string riskLevel, List<string> triggeredRules)
    {
        try
        {
            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                EnableSsl = true
            };

            var subject = $"🚨 FRAUD ALERT - {riskLevel} Risk Transaction";
            var body = GenerateEmailBody(accountId, amount, riskLevel, triggeredRules);

            var mailMessage = new MailMessage(_senderEmail, recipientEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email alert: {ex.Message}");
        }
    }

    private string GenerateEmailBody(string accountId, decimal amount, string riskLevel, List<string> triggeredRules)
    {
        var riskColor = riskLevel switch
        {
            "HIGH" => "#d32f2f",
            "MEDIUM" => "#f57c00",
            _ => "#388e3c"
        };

        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='background-color: {riskColor}; color: white; padding: 20px; border-radius: 5px;'>
                    <h2>⚠️ Fraud Detection Alert</h2>
                </div>

                <div style='padding: 20px; border: 1px solid #ddd; margin-top: 10px;'>
                    <p><strong>Risk Level:</strong> <span style='color: {riskColor}; font-weight: bold;'>{riskLevel}</span></p>
                    <p><strong>Account ID:</strong> {accountId}</p>
                    <p><strong>Transaction Amount:</strong> R {amount:N2}</p>
                    <p><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>

                    <h3>Triggered Rules:</h3>
                    <ul>
                        {string.Join("", triggeredRules.Select(r => $"<li>{r}</li>"))}
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
