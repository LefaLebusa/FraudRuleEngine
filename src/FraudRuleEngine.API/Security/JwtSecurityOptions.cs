namespace FraudRuleEngine.API.Security;

// Bound from the "Authentication" section of appsettings.json (overridden per environment).
// Every value that decides who may call the API comes from here: nothing is hard-coded in Program.cs.
public sealed class JwtSecurityOptions
{
    public const string SectionName = "Authentication";

    // Identity provider that issues the tokens (Dex locally). Must match the token's "iss" claim.
    public string Authority { get; init; } = string.Empty;

    // Audience the token must be issued for (the Dex client id, e.g. "fraud-portal").
    public string Audience { get; init; } = string.Empty;

    // Optional explicit issuer. When empty, the issuer is taken from the provider's discovery document,
    // which is the value the provider actually puts in the token's "iss" claim.
    public string? ValidIssuer { get; init; }

    // Require https for the metadata endpoint. Turn off only in local development (Dex over http).
    public bool RequireHttpsMetadata { get; init; } = true;

    // Tolerance for clock differences between Dex and the API, in seconds.
    public int ClockSkewSeconds { get; init; } = 60;

    // Fails startup with a clear message instead of running with an unsafe or incomplete configuration.
    public void Validate()
    {
        if (!Uri.TryCreate(Authority, UriKind.Absolute, out var authorityUri))
        {
            throw new InvalidOperationException($"{SectionName}:Authority must be an absolute URL, e.g. http://localhost:5556.");
        }

        if (RequireHttpsMetadata && authorityUri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                $"{SectionName}:Authority uses '{authorityUri.Scheme}' but {SectionName}:RequireHttpsMetadata is true. " +
                "Use https, or set RequireHttpsMetadata to false for local development only.");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            throw new InvalidOperationException($"{SectionName}:Audience is required.");
        }

        if (ClockSkewSeconds is < 0 or > 300)
        {
            throw new InvalidOperationException($"{SectionName}:ClockSkewSeconds must be between 0 and 300.");
        }
    }
}
