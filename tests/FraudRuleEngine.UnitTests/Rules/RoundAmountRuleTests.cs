using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Core.Models;
using Xunit;

namespace FraudRuleEngine.UnitTests.Rules;

public class RoundAmountRuleTests
{
    private readonly RoundAmountRule _rule = new();

    [Fact]
    public void Evaluate_ReturnsFalse_WhenAmountBelowMinimum()
    {
        var tx = new Transaction { AccountId = "ACC001", Amount = 1_000m };
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenAmountIsRoundAndAboveMinimum()
    {
        var tx = new Transaction { AccountId = "ACC001", Amount = 10_000m };
        Assert.True(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsFalse_WhenAmountIsNotRound()
    {
        var tx = new Transaction { AccountId = "ACC001", Amount = 10_234.56m };
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenAmountIsExactlyR50000()
    {
        var tx = new Transaction { AccountId = "ACC001", Amount = 50_000m };
        Assert.True(_rule.Evaluate(tx, []));
    }
}
