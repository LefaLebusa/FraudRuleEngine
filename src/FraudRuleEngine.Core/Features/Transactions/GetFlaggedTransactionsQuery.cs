using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.Transactions;

public sealed record GetFlaggedTransactionsQuery(List<string> RiskLevels);

public sealed class GetFlaggedTransactionsQueryHandler(ITransactionRepository transactionRepository)
{
    public async Task<List<Transaction>> Handle(GetFlaggedTransactionsQuery request, CancellationToken cancellationToken)
    {
        return await transactionRepository.GetFlaggedTransactionsAsync(request.RiskLevels, cancellationToken);
    }
}
