using FraudRuleEngine.Application.Rules;

namespace FraudRuleEngine.UnitTests.Rules;

public class DuplicateTransactionRuleTests
{
    private readonly DuplicateTransactionRule _rule = new();

    [Fact]
    public void Evaluate_ReturnsFalse_WhenNoPreviousTransactions()
    {
        var tx = TestTransactions.Create(amount: 500m, merchantName: "ShopRite");
        Assert.False(_rule.Evaluate(tx, []));
    }

    [Fact]
    public void Evaluate_ReturnsTrue_WhenSameAmountAndMerchantWithinWindow()
    {
        var now = DateTime.UtcNow;
        var tx = TestTransactions.Create(amount: 500m, merchantName: "ShopRite", timestamp: now);
        var history = new[] { TestTransactions.Create(amount: 500m, merchantName: "ShopRite", timestamp: now.AddSeconds(-30)) };

        Assert.True(_rule.Evaluate(tx, history));
    }

    [Fact]
    public void Evaluate_ReturnsFalse_WhenSameAmountButDifferentMerchant()
    {
        var now = DateTime.UtcNow;
        var tx = TestTransactions.Create(amount: 500m, merchantName: "Checkers", timestamp: now);
        var history = new[] { TestTransactions.Create(amount: 500m, merchantName: "ShopRite", timestamp: now.AddSeconds(-30)) };

        Assert.False(_rule.Evaluate(tx, history));
    }
}
