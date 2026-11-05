using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.Transactions;

public sealed record GetRecentByAccountQuery(string AccountId, TimeSpan EvaluationWindow);

public sealed class GetRecentByAccountQueryHandler(ITransactionRepository transactionRepository)
{
    public async Task<List<Transaction>> Handle(GetRecentByAccountQuery request, CancellationToken cancellationToken)
    {
        return await transactionRepository.GetRecentByAccountIdAsync(request.AccountId, request.EvaluationWindow, cancellationToken);
    }
}
