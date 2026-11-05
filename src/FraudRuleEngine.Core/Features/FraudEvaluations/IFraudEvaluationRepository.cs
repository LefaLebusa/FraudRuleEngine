using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.FraudEvaluations;

public interface IFraudEvaluationRepository
{
    Task AddEvaluationAsync(FraudEvaluation evaluation, CancellationToken cancellationToken);
}
