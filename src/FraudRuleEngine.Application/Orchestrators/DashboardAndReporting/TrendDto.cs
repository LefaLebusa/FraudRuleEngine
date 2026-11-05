namespace FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;

public sealed class TrendDto
{
    public string Date { get; set; } = string.Empty;
    public int Total { get; set; }
    public double AvgAmount { get; set; }
    public int Flagged { get; set; }
}
