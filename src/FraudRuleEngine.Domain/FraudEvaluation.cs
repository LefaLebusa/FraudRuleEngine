namespace FraudRuleEngine.Domain;

public sealed class FraudEvaluation
{
    private FraudEvaluation() { } // Private constructor to enforce the use of the factory method

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TransactionId { get; private set; }
    public bool IsFlagged { get; private set; }
    public List<string> TriggeredRules { get; private set; } = [];
    public string RiskLevel { get; private set; } = RiskLevels.Low;
    public DateTime EvaluatedAt { get; private set; } = DateTime.UtcNow;

    public Transaction Transaction { get; set; } = default!;

    public static FraudEvaluation Create(Guid transactionId, List<string> triggeredRules)
    {
        return new FraudEvaluation
        {
            TransactionId = transactionId,
            IsFlagged = triggeredRules.Count > 0,
            TriggeredRules = triggeredRules,
            EvaluatedAt = DateTime.UtcNow,
            RiskLevel = triggeredRules.Count switch
            {
                0 => RiskLevels.Low,
                1 => RiskLevels.Medium,
                _ => RiskLevels.High
            }
        };
    }
}
