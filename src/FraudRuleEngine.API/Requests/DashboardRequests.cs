using FluentValidation;

namespace FraudRuleEngine.API.Requests;

// Query string of GET api/dashboard/recent-flagged
public sealed record RecentFlaggedRequest(int Limit = 10);

public sealed class RecentFlaggedRequestValidator : AbstractValidator<RecentFlaggedRequest>
{
    public RecentFlaggedRequestValidator()
    {
        RuleFor(x => x.Limit).InclusiveBetween(1, 100);
    }
}

// Query string of GET api/dashboard/trends
public sealed record TrendsRequest(int Days = 7);

public sealed class TrendsRequestValidator : AbstractValidator<TrendsRequest>
{
    public TrendsRequestValidator()
    {
        RuleFor(x => x.Days).InclusiveBetween(1, 365);
    }
}
