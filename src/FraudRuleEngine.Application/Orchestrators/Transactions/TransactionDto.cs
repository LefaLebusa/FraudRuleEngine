using System.ComponentModel.DataAnnotations;

namespace FraudRuleEngine.Application.Orchestrators.Transactions;

// Alert recipients are NOT part of a transaction submission. They are managed through
// the authenticated account-email endpoint so a caller cannot redirect fraud alerts.
public sealed record TransactionDto(
    [Required, StringLength(50, MinimumLength = 1)]
    string AccountId,

    [Range(0.01, 1000000000)]
    decimal Amount,

    [Required, RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code.")]
    string Currency,

    [Required, StringLength(200, MinimumLength = 1)]
    string MerchantName,

    [Required, StringLength(100, MinimumLength = 1)]
    string MerchantCategory,

    [Required, RegularExpression("^[A-Z]{2}$", ErrorMessage = "Country must be a 2-letter ISO code.")]
    string Country,

    [Required, RegularExpression("^(DEBIT|CREDIT)$", ErrorMessage = "TransactionType must be DEBIT or CREDIT.")]
    string TransactionType,

    [Required, StringLength(64, MinimumLength = 1)]
    string ReferenceNumber
);
