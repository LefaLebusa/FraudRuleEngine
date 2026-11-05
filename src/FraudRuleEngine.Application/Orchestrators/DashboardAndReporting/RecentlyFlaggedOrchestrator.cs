using FraudRuleEngine.Core.Features.Transactions;

namespace FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;

public sealed record RecentlyFlaggedOrchestrator(List<string> RiskLevels, int Size=10);

public sealed class RecentlyFlaggedOrchestratorHandler(GetFlaggedTransactionsQueryHandler flaggedTransactionsHandler)
{
    public async Task<IEnumerable<RecentFlaggedDto>> Handle(RecentlyFlaggedOrchestrator request, CancellationToken cancellationToken)
    {
        var flagged = await flaggedTransactionsHandler.Handle(new GetFlaggedTransactionsQuery(request.RiskLevels), cancellationToken);

        return flagged
            .OrderByDescending(f => f.Timestamp)
            .Take(request.Size)
            .Select(f => new RecentFlaggedDto
            {
                Id = f.Id,
                AccountId = f.AccountId,
                Amount = f.Amount,
                MerchantName = f.MerchantName,
                Timestamp = f.Timestamp,
                RiskLevel = f.FraudEvaluation?.RiskLevel,
                TriggeredRules = f.FraudEvaluation?.TriggeredRules.ToList() ?? new List<string>()
            });
    }
}