using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;
using Microsoft.Extensions.Logging;

namespace FraudRuleEngine.Application.Orchestrators.Transactions;

public sealed record CreateTransactionOrchestrator(IReadOnlyList<TransactionDto> Transactions);

public sealed class CreateTransactionOrchestratorHandler(StoreTransactionCommandHandler saveTransactionCommand,
                                                         ILogger<CreateTransactionOrchestratorHandler> logger)
{
    public async Task<IReadOnlyList<Guid>> Handle(CreateTransactionOrchestrator request, CancellationToken cancellationToken)
    {
        // 1. Create the transactions
        var transactions = request.Transactions
            .Select(dto => Transaction.Create(dto.AccountId,
                                              dto.Amount,
                                              dto.Currency,
                                              dto.MerchantName,
                                              dto.MerchantCategory,
                                              dto.Country,
                                              dto.TransactionType,
                                              dto.ReferenceNumber))
            .ToList();

        // 2. Save the whole batch
        logger.LogDebug("Storing a batch of {TransactionCount} transaction(s)", transactions.Count);
        await saveTransactionCommand.Handle(new StoreTransactionCommand(transactions), cancellationToken);
        logger.LogInformation("Saved {TransactionCount} transaction(s)", transactions.Count);

        return transactions.Select(t => t.Id).ToList();
    }
}
