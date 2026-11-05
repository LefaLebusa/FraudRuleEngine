using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.Transactions;

public sealed record GetByAccountQuery(string AccountId);

public sealed class GetByAccountQueryHandler(ITransactionRepository transactionRepository)
{
    public async Task<List<Transaction>> Handle(GetByAccountQuery request, CancellationToken cancellationToken)
    {
        return await transactionRepository.GetByAccountIdAsync(request.AccountId, cancellationToken);
    }
}
