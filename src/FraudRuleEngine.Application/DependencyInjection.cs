using FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;
using FraudRuleEngine.Application.Orchestrators.Evaluations;
using FraudRuleEngine.Application.Orchestrators.Transactions;
using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Core.Features.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace FraudRuleEngine.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            // Fraud rules
            .AddScoped<IFraudRule, HighAmountRule>()
            .AddScoped<IFraudRule, VelocityRule>()
            .AddScoped<IFraudRule, DuplicateTransactionRule>()
            .AddScoped<IFraudRule, UnusualHourRule>()
            .AddScoped<IFraudRule, RoundAmountRule>()
            // Orchestrators
            .AddScoped<CreateTransactionOrchestratorHandler>()
            .AddScoped<EvaluationOrchestratorHandler>()
            .AddScoped<DashboardSummaryOrchestratorHandler>()
            .AddScoped<RecentlyFlaggedOrchestratorHandler>()
            .AddScoped<TrendAnalysisOrchestratorHandler>()
            .AddScoped<MerchantRiskOrchestratorHandler>();

        return services;
    }
}
