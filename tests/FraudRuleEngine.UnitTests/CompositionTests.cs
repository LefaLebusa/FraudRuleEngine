using FraudRuleEngine.Application;
using FraudRuleEngine.Core;
using FraudRuleEngine.Core.Features.Rules;
using FraudRuleEngine.Infrastructure;
using FraudRuleEngine.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FraudRuleEngine.UnitTests;

// Builds the same wiring as Program.cs, without a database connection.
public class CompositionTests
{
    private static ServiceProvider BuildProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["Email:SmtpServer"] = "smtp.test",
                ["Email:SmtpPort"] = "587",
                ["Email:SenderEmail"] = "alerts@test",
                ["Email:SenderPassword"] = "test"
            })
            .Build();

        return new ServiceCollection()
            .AddLogging()
            .AddCore()
            .AddApplication()
            .AddInfrastructure(configuration)
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    [Fact]
    public void AllServices_CanBeResolved()
    {
        using var provider = BuildProvider();
        Assert.NotNull(provider);
    }

    [Fact]
    public void AllFiveRules_ReachTheEvaluator()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var rules = scope.ServiceProvider.GetServices<IFraudRule>().Select(r => r.RuleName).ToList();

        Assert.Equal(["HIGH_AMOUNT", "HIGH_VELOCITY", "DUPLICATE_TRANSACTION", "UNUSUAL_HOUR", "ROUND_AMOUNT"], rules);
    }

    [Fact]
    public void EfModel_Builds()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var model = scope.ServiceProvider.GetRequiredService<FraudDbContext>().Model;

        Assert.Equal(3, model.GetEntityTypes().Count());
    }

    [Fact]
    public void MissingEmailPassword_FailsAtStartup()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost",
                ["Email:SmtpServer"] = "smtp.test",
                ["Email:SmtpPort"] = "587",
                ["Email:SenderEmail"] = "alerts@test"
            })
            .Build();

        var ex = Assert.Throws<InvalidOperationException>(() => new ServiceCollection().AddInfrastructure(configuration));
        Assert.Contains("Email:SenderPassword", ex.Message);
    }
}
