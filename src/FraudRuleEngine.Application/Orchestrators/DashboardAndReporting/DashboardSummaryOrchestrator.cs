using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;

public sealed record DashboardSummaryOrchestrator();

public sealed class DashboardSummaryOrchestratorHandler(GetFlaggedTransactionsQueryHandler flaggedTransactionsHandler,
                                                        GetAllTransactionsQueryHandler allTransactionsHandler)
{
    public async Task<DashboardSummaryDto> Handle(DashboardSummaryOrchestrator request, CancellationToken cancellationToken)
    {
        var flagged = await flaggedTransactionsHandler.Handle(new GetFlaggedTransactionsQuery([RiskLevels.Medium, RiskLevels.High]), cancellationToken);
        var all = await allTransactionsHandler.Handle(new GetAllTransactionsQuery(), cancellationToken);

        return new DashboardSummaryDto
        {
            TotalTransactions = all.Count,
            FlaggedCount = flagged.Count,
            FlaggedPercentage = all.Count > 0
                ? Math.Round((double)flagged.Count / all.Count * 100, 2)
                : 0,
            // Distribution covers every evaluated transaction, so LOW is no longer always 0
            RiskDistribution = new RiskDistributionDto
            {
                Low = all.Count(t => t.FraudEvaluation?.RiskLevel == RiskLevels.Low),
                Medium = all.Count(t => t.FraudEvaluation?.RiskLevel == RiskLevels.Medium),
                High = all.Count(t => t.FraudEvaluation?.RiskLevel == RiskLevels.High)
            }
        };
    }
}
