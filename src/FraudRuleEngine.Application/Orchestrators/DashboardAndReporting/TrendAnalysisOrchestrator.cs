using FraudRuleEngine.Core.Features.Transactions;

namespace FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;

public sealed record TrendAnalysisOrchestrator(int Days = 7);

public sealed class TrendAnalysisOrchestratorHandler(GetAllTransactionsQueryHandler allTransactionsHandler)
{
    public async Task<IEnumerable<TrendDto>> Handle(TrendAnalysisOrchestrator request, CancellationToken cancellationToken)
    {
        var transactions = await allTransactionsHandler.Handle(new GetAllTransactionsQuery(), cancellationToken);
        var cutoff = DateTime.UtcNow.AddDays(-request.Days);

        return transactions
            .Where(t => t.Timestamp >= cutoff)
            .GroupBy(t => t.Timestamp.Date)
            .OrderBy(g => g.Key)
            .Select(g => new TrendDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Total = g.Count(),
                AvgAmount = g.Average(t => (double)t.Amount),
                Flagged = g.Count(t => t.FraudEvaluation?.IsFlagged ?? false)
            })
            .ToList();
    }
}
