namespace FraudRuleEngine.Application.Orchestrators.DashboardAndReporting;

public sealed record RecentFlaggedDto
{
    public Guid Id { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string MerchantName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? RiskLevel { get; set; }
    public List<string> TriggeredRules { get; set; } = new List<string>();
}
