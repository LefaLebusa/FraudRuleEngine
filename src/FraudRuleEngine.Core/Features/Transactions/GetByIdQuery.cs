using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.Transactions;

public sealed record GetByIdQuery(Guid TransactionId);

public sealed class GetByIdQueryHandler(ITransactionRepository transactionRepository)
{
    public async Task<Transaction?> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        return await transactionRepository.GetByIdAsync(request.TransactionId, cancellationToken);
    }
}
