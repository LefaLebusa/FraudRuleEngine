namespace FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;

public sealed class MerchantRiskDto
{
    public string Merchant { get; set; } = string.Empty;
    public int FlaggedCount { get; set; }
    public int HighRisk { get; set; }
}
