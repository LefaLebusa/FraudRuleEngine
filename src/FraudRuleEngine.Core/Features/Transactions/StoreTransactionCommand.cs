using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.Transactions;

public sealed record StoreTransactionCommand(IReadOnlyList<Transaction> Transactions);

public sealed class StoreTransactionCommandHandler(ITransactionRepository transactionRepository)
{
    public async Task Handle(StoreTransactionCommand request, CancellationToken cancellationToken)
    {
        await transactionRepository.AddRangeAsync(request.Transactions, cancellationToken);
    }
}
