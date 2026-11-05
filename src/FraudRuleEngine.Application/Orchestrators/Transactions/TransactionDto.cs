namespace FraudRuleEngine.Application.Orchestrators.Transactions;

// Alert recipients are NOT part of a transaction submission. They are managed through
// the authenticated account-email endpoint so a caller cannot redirect fraud alerts.
// The rules for each field are in TransactionDtoValidator.
public sealed record TransactionDto(
    string AccountId,
    decimal Amount,
    string Currency,
    string MerchantName,
    string MerchantCategory,
    string Country,
    string TransactionType,
    string ReferenceNumber
);
