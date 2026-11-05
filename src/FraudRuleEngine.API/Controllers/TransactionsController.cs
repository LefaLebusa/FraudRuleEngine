using FluentValidation;
using FraudRuleEngine.API.Requests;
using FraudRuleEngine.API.Validation;
using FraudRuleEngine.Application.Orchestrators.Evaluations;
using FraudRuleEngine.Application.Orchestrators.Transactions;
using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FraudRuleEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(
    CreateTransactionOrchestratorHandler createTransactions,
    EvaluationOrchestratorHandler evaluation,
    GetByIdQueryHandler getById,
    GetFlaggedTransactionsQueryHandler getFlagged,
    GetByAccountQueryHandler getByAccount,
    IValidator<TransactionBatch> transactionBatchValidator,
    IValidator<AccountIdRequest> accountIdValidator) : ControllerBase
{
    private static readonly List<string> FlaggedRiskLevels = [RiskLevels.Medium, RiskLevels.High];

    /// <summary>Submit one or more transactions (1 to 100) and evaluate each one for potential fraud.</summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] List<TransactionDto> request, CancellationToken cancellationToken)
    {
        var invalid = await this.ValidateAsync(transactionBatchValidator, new TransactionBatch(request), cancellationToken);
        if (invalid is not null) return invalid;

        // Save the whole batch first, then evaluate each stored transaction in the order submitted
        var transactionIds = await createTransactions.Handle(new CreateTransactionOrchestrator(request), cancellationToken);
        foreach (var transactionId in transactionIds)
        {
            await evaluation.Handle(new EvaluationOrchestrator(transactionId), cancellationToken);
        }

        return StatusCode(StatusCodes.Status201Created, new { transactionIds });
    }

    /// <summary>Get transaction details.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await getById.Handle(new GetByIdQuery(id), cancellationToken);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    /// <summary>List transactions flagged at Medium or High risk.</summary>
    [HttpGet("flagged")]
    public async Task<IActionResult> GetFlagged(CancellationToken cancellationToken)
        => Ok(await getFlagged.Handle(new GetFlaggedTransactionsQuery(FlaggedRiskLevels), cancellationToken));

    /// <summary>List transactions for an account.</summary>
    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetByAccount(string accountId, CancellationToken cancellationToken)
    {
        var invalid = await this.ValidateAsync(accountIdValidator, new AccountIdRequest(accountId), cancellationToken);
        if (invalid is not null) return invalid;

        return Ok(await getByAccount.Handle(new GetByAccountQuery(accountId), cancellationToken));
    }
}
