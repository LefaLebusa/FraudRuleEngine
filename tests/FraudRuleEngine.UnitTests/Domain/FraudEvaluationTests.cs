using FraudRuleEngine.Domain;

namespace FraudRuleEngine.UnitTests.Domain;

public class FraudEvaluationTests
{
    [Theory]
    [InlineData(0, RiskLevels.Low, false)]
    [InlineData(1, RiskLevels.Medium, true)]
    [InlineData(2, RiskLevels.High, true)]
    [InlineData(4, RiskLevels.High, true)]
    public void Create_SetsRiskLevelFromTriggeredRuleCount(int ruleCount, string expectedRiskLevel, bool expectedFlagged)
    {
        var rules = Enumerable.Range(1, ruleCount).Select(i => $"RULE_{i}").ToList();

        var evaluation = FraudEvaluation.Create(Guid.NewGuid(), rules);

        Assert.Equal(expectedRiskLevel, evaluation.RiskLevel);
        Assert.Equal(expectedFlagged, evaluation.IsFlagged);
    }

    [Fact]
    public void TransactionWithEvaluation_CanBeFormattedWithoutRecursing()
    {
        var transaction = TestTransactions.Create();
        var evaluation = FraudEvaluation.Create(transaction.Id, ["HIGH_AMOUNT"]);
        transaction.FraudEvaluation = evaluation;
        evaluation.Transaction = transaction;

        Assert.NotNull(transaction.ToString());
        Assert.Equal(transaction, transaction);
    }
}
