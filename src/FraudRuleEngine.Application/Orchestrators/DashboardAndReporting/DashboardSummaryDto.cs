namespace FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;

public sealed class DashboardSummaryDto
{
    public int TotalTransactions { get; set; }
    public int FlaggedCount { get; set; }
    public double FlaggedPercentage { get; set; }
    public RiskDistributionDto RiskDistribution { get; set; } = new();
}

public sealed class RiskDistributionDto
{
    public int Low { get; set; }
    public int Medium { get; set; }
    public int High { get; set; }
}
