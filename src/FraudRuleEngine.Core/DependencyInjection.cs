using FraudRuleEngine.Core.Features.AccountEmails;
using FraudRuleEngine.Core.Features.Alertting;
using FraudRuleEngine.Core.Features.FraudEvaluations;
using FraudRuleEngine.Core.Features.Rules;
using FraudRuleEngine.Core.Features.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace FraudRuleEngine.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services
            // Transactions
            .AddScoped<GetByIdQueryHandler>()
            .AddScoped<GetByAccountQueryHandler>()
            .AddScoped<GetAllTransactionsQueryHandler>()
            .AddScoped<GetRecentByAccountQueryHandler>()
            .AddScoped<GetFlaggedTransactionsQueryHandler>()
            .AddScoped<StoreTransactionCommandHandler>()
            // Fraud evaluation
            .AddScoped<CreateFraudEvaluationCommandHandler>()
            .AddScoped<EvaluateTriggeredRulesQueryHandler>()
            // Account email
            .AddScoped<AddEmailCommandHandler>()
            .AddScoped<GetEmailAddressByAccountIDQueryHandler>()
            // Alerts
            .AddScoped<SendAlertNotificationHandler>();

        return services;
    }
}
