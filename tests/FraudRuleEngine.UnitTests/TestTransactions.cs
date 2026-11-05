using FraudRuleEngine.Domain;

namespace FraudRuleEngine.UnitTests;

internal static class TestTransactions
{
    public static Transaction Create(decimal amount = 100m, string merchantName = "ShopRite", DateTime? timestamp = null, string accountId = "ACC001")
        => Transaction.Create(accountId, amount, "ZAR", merchantName, "Groceries", "ZA", "DEBIT", $"REF-{Guid.NewGuid():N}", timestamp);
}
