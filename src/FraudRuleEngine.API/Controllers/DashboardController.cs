using System.ComponentModel.DataAnnotations;
using FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;
using FraudRuleEngine.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FraudRuleEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(
    DashboardSummaryOrchestratorHandler dashboardSummary,
    RecentlyFlaggedOrchestratorHandler recentlyFlagged,
    TrendAnalysisOrchestratorHandler trendAnalysis,
    MerchantRiskOrchestratorHandler merchantRisk) : ControllerBase
{
    private static readonly List<string> FlaggedRiskLevels = [RiskLevels.Medium, RiskLevels.High];

    /// <summary>Get the dashboard summary.</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        => Ok(await dashboardSummary.Handle(new DashboardSummaryOrchestrator(), cancellationToken));

    /// <summary>Get the most recently flagged transactions.</summary>
    [HttpGet("recent-flagged")]
    public async Task<IActionResult> GetRecentFlagged(CancellationToken cancellationToken, [Range(1, 100)] int limit = 10)
        => Ok(await recentlyFlagged.Handle(new RecentlyFlaggedOrchestrator(FlaggedRiskLevels, limit), cancellationToken));

    /// <summary>Get fraud trends over a number of days.</summary>
    [HttpGet("trends")]
    public async Task<IActionResult> GetTrends(CancellationToken cancellationToken, [Range(1, 365)] int days = 7)
        => Ok(await trendAnalysis.Handle(new TrendAnalysisOrchestrator(days), cancellationToken));

    /// <summary>Get risk levels grouped by merchant.</summary>
    [HttpGet("risk-by-merchant")]
    public async Task<IActionResult> GetRiskByMerchant(CancellationToken cancellationToken)
        => Ok(await merchantRisk.Handle(new MerchantRiskOrchestrator(FlaggedRiskLevels), cancellationToken));
}
