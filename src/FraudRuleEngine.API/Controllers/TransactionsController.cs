using FraudRuleEngine.API.DTOs;
using FraudRuleEngine.Application.Services;
using FraudRuleEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FraudRuleEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _service;

    public TransactionsController(TransactionService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] TransactionRequest request)
    {
        var transaction = new Transaction
        {
            AccountId = request.AccountId,
            Amount = request.Amount,
            Currency = request.Currency,
            MerchantName = request.MerchantName,
            MerchantCategory = request.MerchantCategory,
            Country = request.Country,
            TransactionType = request.TransactionType,
            ReferenceNumber = request.ReferenceNumber,
            Timestamp = DateTime.UtcNow
        };

        var (saved, evaluation) = await _service.ProcessAsync(transaction);
        return CreatedAtAction(nameof(GetById), new { id = saved.Id }, new { saved, evaluation });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _service.GetByIdAsync(id);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpGet("flagged")]
    public async Task<IActionResult> GetFlagged()
        => Ok(await _service.GetFlaggedAsync());

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetByAccount(string accountId)
        => Ok(await _service.GetByAccountAsync(accountId));
}
