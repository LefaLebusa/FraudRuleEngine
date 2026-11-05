using FraudRuleEngine.Application.Orchestrators.Transactions;
using Xunit;

namespace FraudRuleEngine.UnitTests.Validation;

public class TransactionBatchValidatorTests
{
    private static readonly TransactionBatchValidator Validator = new();

    private static TransactionDto Valid() =>
        new("ACC001", 250m, "ZAR", "ShopRite", "Groceries", "ZA", "DEBIT", "REF-1");

    [Fact]
    public void Single_valid_transaction_passes()
    {
        var result = Validator.Validate(new TransactionBatch([Valid()]));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_batch_fails()
    {
        var result = Validator.Validate(new TransactionBatch([]));

        Assert.Contains(result.Errors, e => e.PropertyName == "Transactions");
    }

    [Fact]
    public void Null_list_fails_without_throwing()
    {
        var result = Validator.Validate(new TransactionBatch(null!));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Batch_over_the_limit_fails()
    {
        var batch = Enumerable.Range(0, TransactionBatchValidator.MaxBatchSize + 1).Select(_ => Valid()).ToList();

        var result = Validator.Validate(new TransactionBatch(batch));

        Assert.Contains(result.Errors, e => e.PropertyName == "Transactions");
    }

    [Fact]
    public void Batch_at_the_limit_passes()
    {
        var batch = Enumerable.Range(0, TransactionBatchValidator.MaxBatchSize).Select(_ => Valid()).ToList();

        var result = Validator.Validate(new TransactionBatch(batch));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Invalid_item_is_reported_with_its_position()
    {
        var batch = new TransactionBatch([Valid(), Valid() with { Amount = 0m }]);

        var result = Validator.Validate(batch);

        Assert.Contains(result.Errors, e => e.PropertyName == "Transactions[1].Amount");
    }
}
