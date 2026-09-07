using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Core.Models;
using Xunit;

namespace FraudRuleEngine.UnitTests.Rules;

public class HighAmountRuleTests
{
    private readonly HighAmountRule _rule = new();

    [Fact]
    public void Evaluate_ReturnsFalse_WhenAmountBelowThreshold()
    {
        var tx = new Transaction { AccountId = "ACC001", Amount = 10_000m };
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsFalse_WhenAmountAtThreshold()
    {
        var tx = new Transaction { AccountId = "ACC001", Amount = 50_000m };
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenAmountExceedsThreshold()
    {
        var tx = new Transaction { AccountId = "ACC001", Amount = 50_001m };
        Assert.True(_rule.Evaluate(tx, []));
    }
}
