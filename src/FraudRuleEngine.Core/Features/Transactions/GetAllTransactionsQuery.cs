using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.Transactions;

public sealed record GetAllTransactionsQuery;

public sealed class GetAllTransactionsQueryHandler(ITransactionRepository transactionRepository)
{
    public async Task<List<Transaction>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        return await transactionRepository.GetAllAsync(cancellationToken);
    }
}
