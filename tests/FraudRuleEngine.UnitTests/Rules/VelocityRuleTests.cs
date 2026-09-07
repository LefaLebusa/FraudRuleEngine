using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Core.Models;
using Xunit;

namespace FraudRuleEngine.UnitTests.Rules;

public class VelocityRuleTests
{
    private readonly VelocityRule _rule = new();

    [Fact]
    public void Evaluate_ReturnsFalse_WhenFewTransactionsInWindow()
    {
        var now = DateTime.UtcNow;
        var tx = new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Timestamp = now };
        var history = Enumerable.Range(1, 4)
            .Select(_ => new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Timestamp = now.AddMinutes(-1) });

        Assert.False(_rule.Evaluate(tx, history));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenFiveOrMoreTransactionsInWindow()
    {
        var now = DateTime.UtcNow;
        var tx = new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Timestamp = now };
        var history = Enumerable.Range(1, 5)
            .Select(_ => new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Timestamp = now.AddMinutes(-1) });

        Assert.True(_rule.Evaluate(tx, history));
    }

    [Fact]
    public void Evaluate_ReturnsFalse_WhenTransactionsOutsideWindow()
    {
        var now = DateTime.UtcNow;
        var tx = new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Timestamp = now };
        var history = Enumerable.Range(1, 10)
            .Select(_ => new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Timestamp = now.AddMinutes(-15) });

        Assert.False(_rule.Evaluate(tx, history));
    }
}
