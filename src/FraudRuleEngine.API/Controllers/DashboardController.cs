using FraudRuleEngine.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FraudRuleEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly TransactionService _service;

    public DashboardController(TransactionService service) => _service = service;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var flagged = (await _service.GetFlaggedAsync()).ToList();
        var all = (await _service.GetAllAsync()).ToList();

        var lowRisk = flagged.Count(f => f.FraudEvaluation?.RiskLevel == "LOW");
        var mediumRisk = flagged.Count(f => f.FraudEvaluation?.RiskLevel == "MEDIUM");
        var highRisk = flagged.Count(f => f.FraudEvaluation?.RiskLevel == "HIGH");

        var summary = new
        {
            TotalTransactions = all.Count(),
            FlaggedCount = flagged.Count(),
            FlaggedPercentage = all.Count > 0 ? Math.Round((double)flagged.Count / all.Count * 100, 2) : 0,
            RiskDistribution = new
            {
                Low = lowRisk,
                Medium = mediumRisk,
                High = highRisk
            }
        };

        return Ok(summary);
    }

    [HttpGet("recent-flagged")]
    public async Task<IActionResult> GetRecentFlagged(int limit = 10)
    {
        var flagged = (await _service.GetFlaggedAsync()).ToList();
        var recent = flagged
            .OrderByDescending(f => f.Timestamp)
            .Take(limit)
            .Select(f => new
            {
                f.Id,
                f.AccountId,
                f.Amount,
                f.MerchantName,
                f.Timestamp,
                RiskLevel = f.FraudEvaluation?.RiskLevel,
                TriggeredRules = f.FraudEvaluation?.TriggeredRules
            });

        return Ok(recent);
    }

    [HttpGet("trends")]
    public async Task<IActionResult> GetTrends(int days = 7)
    {
        var all = (await _service.GetAllAsync()).ToList();
        var startDate = DateTime.UtcNow.AddDays(-days);

        var trends = all
            .Where(t => t.Timestamp >= startDate)
            .GroupBy(t => t.Timestamp.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Total = g.Count(),
                AvgAmount = g.Average(t => (double)t.Amount),
                Flagged = g.Count(t => t.FraudEvaluation?.IsFlagged ?? false)
            });

        return Ok(trends);
    }

    [HttpGet("risk-by-merchant")]
    public async Task<IActionResult> GetRiskByMerchant()
    {
        var flagged = (await _service.GetFlaggedAsync()).ToList();
        var byMerchant = flagged
            .GroupBy(f => f.MerchantName)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new
            {
                Merchant = g.Key,
                FlaggedCount = g.Count(),
                HighRisk = g.Count(f => f.FraudEvaluation?.RiskLevel == "HIGH")
            });

        return Ok(byMerchant);
    }
}
