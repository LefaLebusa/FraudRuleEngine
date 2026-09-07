using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Core.Models;
using Xunit;

namespace FraudRuleEngine.UnitTests.Rules;

public class DuplicateTransactionRuleTests
{
    private readonly DuplicateTransactionRule _rule = new();

    [Fact]
    public void Evaluate_ReturnsFalse_WhenNoPreviousTransactions()
    {
        var tx = new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Amount = 500m, MerchantName = "ShopRite", Timestamp = DateTime.UtcNow };
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenSameAmountAndMerchantWithinWindow()
    {
        var now = DateTime.UtcNow;
        var tx = new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Amount = 500m, MerchantName = "ShopRite", Timestamp = now };
        var history = new[]
        {
            new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Amount = 500m, MerchantName = "ShopRite", Timestamp = now.AddSeconds(-30) }
        };

        Assert.True(_rule.Evaluate(tx, history));
    }

    [Fact]
    public void Evaluate_ReturnsFalse_WhenSameAmountButDifferentMerchant()
    {
        var now = DateTime.UtcNow;
        var tx = new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Amount = 500m, MerchantName = "Checkers", Timestamp = now };
        var history = new[]
        {
            new Transaction { Id = Guid.NewGuid(), AccountId = "ACC001", Amount = 500m, MerchantName = "ShopRite", Timestamp = now.AddSeconds(-30) }
        };

        Assert.False(_rule.Evaluate(tx, history));
    }
}
