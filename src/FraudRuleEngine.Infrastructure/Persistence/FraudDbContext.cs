using FraudRuleEngine.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudRuleEngine.Infrastructure.Persistence;

public class FraudDbContext : DbContext
{
    public FraudDbContext(DbContextOptions<FraudDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FraudEvaluation> FraudEvaluations => Set<FraudEvaluation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.HasOne(t => t.FraudEvaluation)
             .WithOne(f => f.Transaction)
             .HasForeignKey<FraudEvaluation>(f => f.TransactionId);
        });

        modelBuilder.Entity<FraudEvaluation>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.TriggeredRules)
             .HasConversion(
                 v => string.Join(',', v),
                 v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        });
    }
}
