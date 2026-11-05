using FluentValidation;

namespace FraudRuleEngine.Application.Orchestrators.Transactions;

// The body of POST api/transactions is a JSON array. It is wrapped in this type so that
// FluentValidation reports errors with the item's position, for example "Transactions[2].Amount".
public sealed record TransactionBatch(List<TransactionDto> Transactions);

// A non-empty batch of at most MaxBatchSize transactions, each validated with TransactionDtoValidator.
public sealed class TransactionBatchValidator : AbstractValidator<TransactionBatch>
{
    public const int MaxBatchSize = 100;

    public TransactionBatchValidator()
    {
        RuleFor(batch => batch.Transactions)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("At least one transaction is required.")
            .Must(transactions => transactions.Count <= MaxBatchSize)
            .WithMessage($"A batch can contain at most {MaxBatchSize} transactions.");

        RuleForEach(batch => batch.Transactions).SetValidator(new TransactionDtoValidator());
    }
}
