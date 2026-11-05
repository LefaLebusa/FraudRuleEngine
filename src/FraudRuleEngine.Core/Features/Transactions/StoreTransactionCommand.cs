using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.Transactions;

public sealed record StoreTransactionCommand(Transaction Transaction);

public sealed class StoreTransactionCommandHandler(ITransactionRepository transactionRepository)
{
    public async Task Handle(StoreTransactionCommand request, CancellationToken cancellationToken)
    {
        await transactionRepository.AddAsync(request.Transaction, cancellationToken);
    }
}
