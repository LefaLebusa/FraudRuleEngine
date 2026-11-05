 namespace FraudRuleEngine.Application.Orchestrators.Transactions;

 public sealed record TransactionDetailDto(
     Guid Id,
     string AccountId,
     decimal Amount,
     string Currency,
     string MerchantName,
     string MerchantCategory,
     string Country,
     string TransactionType,
     string ReferenceNumber
 );