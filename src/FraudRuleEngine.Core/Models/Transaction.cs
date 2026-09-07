namespace FraudRuleEngine.Core.Models;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AccountId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string MerchantName { get; set; } = default!;
    public string MerchantCategory { get; set; } = default!;
    public string Country { get; set; } = "ZA";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TransactionType { get; set; } = default!; // DEBIT / CREDIT
    public string ReferenceNumber { get; set; } = default!;

    public FraudEvaluation? FraudEvaluation { get; set; }
}
