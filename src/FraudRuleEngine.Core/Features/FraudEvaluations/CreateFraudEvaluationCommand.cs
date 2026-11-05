using FraudRuleEngine.Domain;

namespace FraudRuleEngine.Core.Features.FraudEvaluations;

public sealed record CreateFraudEvaluationCommand(FraudEvaluation FraudEvaluation);

public sealed class CreateFraudEvaluationCommandHandler(IFraudEvaluationRepository fraudEvaluationRepository)
{
    public async Task Handle(CreateFraudEvaluationCommand request, CancellationToken cancellationToken)
    {
        await fraudEvaluationRepository.AddEvaluationAsync(request.FraudEvaluation, cancellationToken);
    }
}
