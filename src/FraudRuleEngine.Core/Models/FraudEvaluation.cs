namespace FraudRuleEngine.Core.Models;

public class FraudEvaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TransactionId { get; set; }
    public bool IsFlagged { get; set; }
    public List<string> TriggeredRules { get; set; } = [];
    public string RiskLevel { get; set; } = "LOW"; // LOW / MEDIUM / HIGH
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    public Transaction Transaction { get; set; } = default!;
}
