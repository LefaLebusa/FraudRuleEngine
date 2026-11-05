using FraudRuleEngine.Application.Orchestrators.Evaluations;
using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Core.Features.AccountEmails;
using FraudRuleEngine.Core.Features.Alertting;
using FraudRuleEngine.Core.Features.FraudEvaluations;
using FraudRuleEngine.Core.Features.Rules;
using FraudRuleEngine.Core.Features.Transactions;
using FraudRuleEngine.Domain;
using Microsoft.Extensions.Logging.Abstractions;

namespace FraudRuleEngine.UnitTests.Orchestrators;

public class EvaluationOrchestratorTests
{
    // 10:00 SAST, so UNUSUAL_HOUR never fires in these tests
    private static readonly DateTime BusinessHoursUtc = DateTime.SpecifyKind(DateTime.Today.AddHours(8), DateTimeKind.Utc);

    private readonly FakeTransactionRepository _transactions = new();
    private readonly FakeFraudEvaluationRepository _evaluations = new();
    private readonly FakeAccountEmailRepository _emails = new();
    private readonly FakeEmailService _emailService = new();
    private readonly EvaluationOrchestratorHandler _handler;

    public EvaluationOrchestratorTests()
    {
        IFraudRule[] rules = [new HighAmountRule(), new VelocityRule(), new DuplicateTransactionRule(), new UnusualHourRule(), new RoundAmountRule()];

        _handler = new EvaluationOrchestratorHandler(
            new GetByIdQueryHandler(_transactions),
            new GetRecentByAccountQueryHandler(_transactions),
            new CreateFraudEvaluationCommandHandler(_evaluations),
            new EvaluateTriggeredRulesQueryHandler(rules, NullLogger<EvaluateTriggeredRulesQueryHandler>.Instance),
            new SendAlertNotificationHandler(_emailService,
                                             new GetEmailAddressByAccountIDQueryHandler(_emails),
                                             NullLogger<SendAlertNotificationHandler>.Instance),
            NullLogger<EvaluationOrchestratorHandler>.Instance);

        _emails.Emails["ACC001"] = "customer@example.com";
    }

    private async Task<FraudEvaluation> Evaluate(decimal amount)
    {
        var transaction = TestTransactions.Create(amount: amount, timestamp: BusinessHoursUtc);
        _transactions.Items.Add(transaction);
        return await _handler.Handle(new EvaluationOrchestrator(transaction.Id), CancellationToken.None);
    }

    [Fact]
    public async Task CleanTransaction_IsLowRisk_AndSendsNoAlert()
    {
        var evaluation = await Evaluate(123.45m);

        Assert.False(evaluation.IsFlagged);
        Assert.Equal(RiskLevels.Low, evaluation.RiskLevel);
        Assert.Single(_evaluations.Saved);
        Assert.Empty(_emailService.Sent);
    }

    [Fact]
    public async Task OneRuleTriggered_IsMediumRisk_AndSendsNoAlert()
    {
        var evaluation = await Evaluate(60_000.50m); // HIGH_AMOUNT only

        Assert.Equal(["HIGH_AMOUNT"], evaluation.TriggeredRules);
        Assert.Equal(RiskLevels.Medium, evaluation.RiskLevel);
        Assert.Empty(_emailService.Sent);
    }

    [Fact]
    public async Task TwoRulesTriggered_IsHighRisk_AndSendsOneAlert()
    {
        var evaluation = await Evaluate(60_000m); // HIGH_AMOUNT + ROUND_AMOUNT

        Assert.Equal(RiskLevels.High, evaluation.RiskLevel);
        Assert.Equal([("customer@example.com", RiskLevels.High)], _emailService.Sent);
    }

    [Fact]
    public async Task HighRisk_WithNoRegisteredEmail_DoesNotThrowOrSend()
    {
        _emails.Emails.Clear();

        var evaluation = await Evaluate(60_000m);

        Assert.Equal(RiskLevels.High, evaluation.RiskLevel);
        Assert.Empty(_emailService.Sent);
    }

    [Fact]
    public async Task UnknownTransaction_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(new EvaluationOrchestrator(Guid.NewGuid()), CancellationToken.None));
    }
}
