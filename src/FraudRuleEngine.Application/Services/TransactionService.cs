using FraudRuleEngine.Core.Interfaces;
using FraudRuleEngine.Core.Models;

namespace FraudRuleEngine.Application.Services;

public class TransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly FraudEvaluationService _evaluationService;

    public TransactionService(ITransactionRepository repository, FraudEvaluationService evaluationService)
    {
        _repository = repository;
        _evaluationService = evaluationService;
    }

    public async Task<(Transaction, FraudEvaluation)> ProcessAsync(Transaction transaction)
    {
        await _repository.AddAsync(transaction);
        var evaluation = await _evaluationService.EvaluateAsync(transaction);
        return (transaction, evaluation);
    }

    public Task<Transaction?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

    public Task<IEnumerable<Transaction>> GetFlaggedAsync() => _repository.GetFlaggedAsync();

    public Task<IEnumerable<Transaction>> GetByAccountAsync(string accountId)
        => _repository.GetByAccountIdAsync(accountId);
}
