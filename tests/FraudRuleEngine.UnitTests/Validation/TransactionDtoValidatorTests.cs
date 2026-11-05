using FraudRuleEngine.Application.Orchestrators.Transactions;
using Xunit;

namespace FraudRuleEngine.UnitTests.Validation;

public class TransactionDtoValidatorTests
{
    private static readonly TransactionDtoValidator Validator = new();

    private static TransactionDto Valid() =>
        new("ACC001", 250m, "ZAR", "ShopRite", "Groceries", "ZA", "DEBIT", "REF-1");

    [Fact]
    public void Valid_transaction_passes()
    {
        var result = Validator.Validate(Valid());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(1_000_000_001)]
    public void Amount_outside_allowed_range_fails(double amount)
    {
        var result = Validator.Validate(Valid() with { Amount = (decimal)amount });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TransactionDto.Amount));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("ZA")]
    [InlineData("zar")]
    public void Invalid_currency_fails(string? currency)
    {
        var result = Validator.Validate(Valid() with { Currency = currency! });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TransactionDto.Currency));
    }

    [Theory]
    [InlineData("za")]
    [InlineData("ZAF")]
    public void Invalid_country_fails(string country)
    {
        var result = Validator.Validate(Valid() with { Country = country });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TransactionDto.Country));
    }

    [Theory]
    [InlineData("REFUND")]
    [InlineData("debit")]
    public void Transaction_type_must_be_debit_or_credit(string type)
    {
        var result = Validator.Validate(Valid() with { TransactionType = type });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TransactionDto.TransactionType));
    }

    [Fact]
    public void Account_id_longer_than_50_characters_fails()
    {
        var result = Validator.Validate(Valid() with { AccountId = new string('A', 51) });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TransactionDto.AccountId));
    }

    [Fact]
    public void Missing_required_text_fields_are_reported_together()
    {
        var result = Validator.Validate(Valid() with { MerchantName = "", ReferenceNumber = "" });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TransactionDto.MerchantName));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TransactionDto.ReferenceNumber));
    }
}
