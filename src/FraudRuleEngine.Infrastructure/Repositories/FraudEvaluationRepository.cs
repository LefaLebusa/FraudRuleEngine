using FraudRuleEngine.Core.Features.FraudEvaluations;
using FraudRuleEngine.Domain;
using FraudRuleEngine.Infrastructure.Persistence;

namespace FraudRuleEngine.Infrastructure.Repositories;

public class FraudEvaluationRepository(FraudDbContext context) : IFraudEvaluationRepository
{
    public async Task AddEvaluationAsync(FraudEvaluation evaluation, CancellationToken cancellationToken)
    {
        context.FraudEvaluations.Add(evaluation);
        await context.SaveChangesAsync(cancellationToken);
    }
}
