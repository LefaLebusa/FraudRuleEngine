using FluentValidation;
using FraudRuleEngine.API.Requests;
using FraudRuleEngine.API.Validation;
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
    MerchantRiskOrchestratorHandler merchantRisk,
    IValidator<RecentFlaggedRequest> recentFlaggedValidator,
    IValidator<TrendsRequest> trendsValidator) : ControllerBase
{
    private static readonly List<string> FlaggedRiskLevels = [RiskLevels.Medium, RiskLevels.High];

    /// <summary>Get the dashboard summary.</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        => Ok(await dashboardSummary.Handle(new DashboardSummaryOrchestrator(), cancellationToken));

    /// <summary>Get the most recently flagged transactions.</summary>
    [HttpGet("recent-flagged")]
    public async Task<IActionResult> GetRecentFlagged([FromQuery] RecentFlaggedRequest request, CancellationToken cancellationToken)
    {
        var invalid = await this.ValidateAsync(recentFlaggedValidator, request, cancellationToken);
        if (invalid is not null) return invalid;

        return Ok(await recentlyFlagged.Handle(new RecentlyFlaggedOrchestrator(FlaggedRiskLevels, request.Limit), cancellationToken));
    }

    /// <summary>Get fraud trends over a number of days.</summary>
    [HttpGet("trends")]
    public async Task<IActionResult> GetTrends([FromQuery] TrendsRequest request, CancellationToken cancellationToken)
    {
        var invalid = await this.ValidateAsync(trendsValidator, request, cancellationToken);
        if (invalid is not null) return invalid;

        return Ok(await trendAnalysis.Handle(new TrendAnalysisOrchestrator(request.Days), cancellationToken));
    }

    /// <summary>Get risk levels grouped by merchant.</summary>
    [HttpGet("risk-by-merchant")]
    public async Task<IActionResult> GetRiskByMerchant(CancellationToken cancellationToken)
        => Ok(await merchantRisk.Handle(new MerchantRiskOrchestrator(FlaggedRiskLevels), cancellationToken));
}
