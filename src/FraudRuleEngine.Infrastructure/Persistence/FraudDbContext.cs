using FraudRuleEngine.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FraudRuleEngine.Infrastructure.Persistence;

public class FraudDbContext : DbContext
{
    public FraudDbContext(DbContextOptions<FraudDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FraudEvaluation> FraudEvaluations => Set<FraudEvaluation>();
    public DbSet<AccountEmail> AccountEmails => Set<AccountEmail>();

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

        var stringListConverter = new ValueConverter<List<string>, string>(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<FraudEvaluation>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.TriggeredRules)
             .HasConversion(stringListConverter)
             .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                 (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                 c => c.ToList()));
        });

        modelBuilder.Entity<AccountEmail>(e =>
        {
            e.HasKey(a => a.AccountId);
            e.Property(a => a.Email).IsRequired();
        });
    }
}
