using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;
using Microsoft.Extensions.Logging;

namespace FraudRuleEngine.Application.Orchestrators.Transactions;

public sealed record CreateTransactionOrchestrator(TransactionDto Transaction);

public sealed class CreateTransactionOrchestratorHandler(StoreTransactionCommandHandler saveTransactionCommand,
                                                         ILogger<CreateTransactionOrchestratorHandler> logger)
{
    public async Task<Guid> Handle(CreateTransactionOrchestrator request, CancellationToken cancellationToken)
    {
        // 1. Create the transaction
        var transaction = Transaction.Create(request.Transaction.AccountId,
                                             request.Transaction.Amount,
                                             request.Transaction.Currency,
                                             request.Transaction.MerchantName,
                                             request.Transaction.MerchantCategory,
                                             request.Transaction.Country,
                                             request.Transaction.TransactionType,
                                             request.Transaction.ReferenceNumber);

        // 2. Save the transaction
        logger.LogDebug("Storing {TransactionType} of {Amount} {Currency} for account {AccountId}",
                        transaction.TransactionType, transaction.Amount, transaction.Currency, transaction.AccountId);
        await saveTransactionCommand.Handle(new StoreTransactionCommand(transaction), cancellationToken);
        logger.LogInformation("Transaction saved with ID: {TransactionId}", transaction.Id);

        return transaction.Id;
    }
}
