using System.ComponentModel.DataAnnotations;
using FraudRuleEngine.Application.Orchestrators.Evaluations;
using FraudRuleEngine.Application.Orchestrators.Transactions;
using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FraudRuleEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(
    CreateTransactionOrchestratorHandler createTransaction,
                                    EvaluationOrchestratorHandler evaluation,
                                    GetByIdQueryHandler getById,
                                    GetFlaggedTransactionsQueryHandler getFlagged,
                                    GetByAccountQueryHandler getByAccount) : ControllerBase
{   
    private static readonly List<string> FlaggedRiskLevels = [RiskLevels.Medium, RiskLevels.High];

    /// <summary>Submit a transaction and evaluate it for potential fraud.</summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] TransactionDto request, CancellationToken cancellationToken)
    {
        var transactionId = await createTransaction.Handle(new CreateTransactionOrchestrator(request), cancellationToken);
        await evaluation.Handle(new EvaluationOrchestrator(transactionId), cancellationToken);
        
        return CreatedAtAction(nameof(GetById), new { id = transactionId }, new { id = transactionId });
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
    public async Task<IActionResult> GetByAccount([StringLength(50, MinimumLength = 1)] string accountId, CancellationToken cancellationToken)
        => Ok(await getByAccount.Handle(new GetByAccountQuery(accountId), cancellationToken));
    }
