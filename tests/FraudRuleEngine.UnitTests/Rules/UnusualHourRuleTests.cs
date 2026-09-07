using FraudRuleEngine.Application.Rules;
using FraudRuleEngine.Core.Models;
using Xunit;

namespace FraudRuleEngine.UnitTests.Rules;

public class UnusualHourRuleTests
{
    private readonly UnusualHourRule _rule = new();

    [Theory]
    [InlineData(0)]  // midnight SAST = 22:00 UTC prev day
    [InlineData(1)]
    [InlineData(3)]
    public void Evaluate_ReturnsTrue_WhenTransactionBetweenMidnightAnd4amSAST(int sastHour)
    {
        // SAST = UTC+2, so 1am SAST = 23:00 UTC previous day
        var utcTime = DateTime.SpecifyKind(
            DateTime.Today.AddHours(sastHour - 2), DateTimeKind.Utc);
        var tx = new Transaction { AccountId = "ACC001", Timestamp = utcTime };

        Assert.True(_rule.Evaluate(tx, []));
    }

    [Theory]
    [InlineData(9)]   // 9am SAST
    [InlineData(14)]  // 2pm SAST
    [InlineData(20)]  // 8pm SAST
    public void Evaluate_ReturnsFalse_WhenTransactionDuringBusinessHours(int sastHour)
    {
        var utcTime = DateTime.SpecifyKind(
            DateTime.Today.AddHours(sastHour - 2), DateTimeKind.Utc);
        var tx = new Transaction { AccountId = "ACC001", Timestamp = utcTime };

        Assert.False(_rule.Evaluate(tx, []));
    }
}
