namespace FraudRuleEngine.API.DTOs;

public record TransactionRequest(
    string AccountId,
    decimal Amount,
    string Currency,
    string MerchantName,
    string MerchantCategory,
    string Country,
    string TransactionType,
    string ReferenceNumber
);
