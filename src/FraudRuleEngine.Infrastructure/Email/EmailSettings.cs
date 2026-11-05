namespace FraudRuleEngine.Infrastructure.Email;

public sealed record EmailSettings(string SmtpServer, int SmtpPort, string SenderEmail, string SenderPassword);
