using FraudRuleEngine.Application.Rules;

namespace FraudRuleEngine.UnitTests.Rules;

public class RoundAmountRuleTests
{
    private readonly RoundAmountRule _rule = new();

    [Fact]
    public void Evaluate_ReturnsFalse_WhenAmountBelowMinimum()
    {
        var tx = TestTransactions.Create(amount: 1_000m);
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenAmountIsRoundAndAboveMinimum()
    {
        var tx = TestTransactions.Create(amount: 10_000m);
        Assert.True(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsFalse_WhenAmountIsNotRound()
    {
        var tx = TestTransactions.Create(amount: 10_234.56m);
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenAmountIsExactlyR50000()
    {
        var tx = TestTransactions.Create(amount: 50_000m);
        Assert.True(_rule.Evaluate(tx, []));
    }
}
