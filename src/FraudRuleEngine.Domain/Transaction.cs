namespace FraudRuleEngine.Domain;

// A class, not a record: record equality and ToString would recurse through the FraudEvaluation <-> Transaction navigation.
public sealed class Transaction
{
    private Transaction() { } // Private constructor to enforce the use of the factory method

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string AccountId { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "ZAR";
    public string MerchantName { get; private set; } = default!;
    public string MerchantCategory { get; private set; } = default!;
    public string Country { get; private set; } = "ZA";
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public string TransactionType { get; private set; } = default!; // DEBIT / CREDIT
    public string ReferenceNumber { get; private set; } = default!;

    public FraudEvaluation? FraudEvaluation { get; set; }

    public static Transaction Create(string accountId, decimal amount, string currency, string merchantName, string merchantCategory, string country, string transactionType, string referenceNumber, DateTime? timestamp = null)
    {
        if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentException("AccountId cannot be null or empty.", nameof(accountId));
        if (string.IsNullOrWhiteSpace(merchantName)) throw new ArgumentException("MerchantName cannot be null or empty.", nameof(merchantName));
        if (string.IsNullOrWhiteSpace(merchantCategory)) throw new ArgumentException("MerchantCategory cannot be null or empty.", nameof(merchantCategory));
        if (string.IsNullOrWhiteSpace(transactionType)) throw new ArgumentException("TransactionType cannot be null or empty.", nameof(transactionType));
        if (string.IsNullOrWhiteSpace(referenceNumber)) throw new ArgumentException("ReferenceNumber cannot be null or empty.", nameof(referenceNumber));

        return new Transaction
        {
            AccountId = accountId,
            Amount = amount,
            Currency = currency,
            MerchantName = merchantName,
            MerchantCategory = merchantCategory,
            Country = country,
            TransactionType = transactionType,
            ReferenceNumber = referenceNumber,
            Timestamp = timestamp ?? DateTime.UtcNow
        };
    }
}
