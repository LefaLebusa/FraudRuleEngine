using FraudRuleEngine.Application.Rules;

namespace FraudRuleEngine.UnitTests.Rules;

public class HighAmountRuleTests
{
    private readonly HighAmountRule _rule = new();

    [Fact]
    public void Evaluate_ReturnsFalse_WhenAmountBelowThreshold()
    {
        var tx = TestTransactions.Create(amount: 10_000m);
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsFalse_WhenAmountAtThreshold()
    {
        var tx = TestTransactions.Create(amount: 50_000m);
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenAmountExceedsThreshold()
    {
        var tx = TestTransactions.Create(amount: 50_001m);
        Assert.True(_rule.Evaluate(tx, []));
    }
}
