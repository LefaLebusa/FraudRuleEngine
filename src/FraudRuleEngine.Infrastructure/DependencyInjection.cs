using FraudRuleEngine.Core.Features.AccountEmails;
using FraudRuleEngine.Core.Features.Alertting;
using FraudRuleEngine.Core.Features.FraudEvaluations;
using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Infrastructure.Email;
using FraudRuleEngine.Infrastructure.Persistence;
using FraudRuleEngine.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FraudRuleEngine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

        var email = configuration.GetSection("Email");
        var emailSettings = new EmailSettings(
            Required(email, "SmtpServer"),
            int.Parse(Required(email, "SmtpPort")),
            Required(email, "SenderEmail"),
            Required(email, "SenderPassword"));

        services
            // Persistence
            .AddDbContext<FraudDbContext>(options => options.UseNpgsql(connectionString))
            .AddScoped<ITransactionRepository, TransactionRepository>()
            .AddScoped<IFraudEvaluationRepository, FraudEvaluationRepository>()
            .AddScoped<IAccountEmailRepository, AccountEmailRepository>()
            // Email
            .AddSingleton(emailSettings)
            .AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static string Required(IConfigurationSection section, string key)
        => section[key] ?? throw new InvalidOperationException($"{section.Path}:{key} is not configured.");
}
