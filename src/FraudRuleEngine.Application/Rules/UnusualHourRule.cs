using FraudRuleEngine.Core.Models;
using FraudRuleEngine.Core.Rules;

namespace FraudRuleEngine.Application.Rules;

// Flags transactions between midnight and 4am SAST (UTC+2)
public class UnusualHourRule : IFraudRule
{
    private static readonly TimeZoneInfo Sast = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");

    public string RuleName => "UNUSUAL_HOUR";

    public bool Evaluate(Transaction transaction, IEnumerable<Transaction> accountHistory)
    {
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(transaction.Timestamp, Sast);
        return localTime.Hour is >= 0 and < 4;
    }
}
