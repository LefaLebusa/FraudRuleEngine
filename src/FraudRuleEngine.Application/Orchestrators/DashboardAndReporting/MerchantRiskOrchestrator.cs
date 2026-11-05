using FraudRuleEngine.Domain;
using FraudRuleEngine.Core.Features.Transactions;

namespace FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;

public sealed record MerchantRiskOrchestrator(List<string> RiskLevels, int Top = 10);

public sealed class MerchantRiskOrchestratorHandler(GetFlaggedTransactionsQueryHandler flaggedTransactionsHandler)
{
    public async Task<IEnumerable<MerchantRiskDto>> Handle(MerchantRiskOrchestrator request, CancellationToken cancellationToken)
    {
        var flagged = await flaggedTransactionsHandler.Handle(new GetFlaggedTransactionsQuery(request.RiskLevels), cancellationToken);

        return flagged
            .GroupBy(t => t.MerchantName)
            .Select(g => new MerchantRiskDto
            {
                Merchant = g.Key,
                FlaggedCount = g.Count(),
                HighRisk = g.Count(t => t.FraudEvaluation?.RiskLevel == RiskLevels.High)
            })
            .OrderByDescending(m => m.FlaggedCount)
            .Take(request.Top)
            .ToList();
    }
}
