using System;
using FraudRuleEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FraudRuleEngine.Infrastructure.Migrations
{
    [DbContext(typeof(FraudDbContext))]
    partial class FraudDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FraudRuleEngine.Core.Models.FraudEvaluation", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<DateTime>("EvaluatedAt").HasColumnType("timestamp with time zone");
                b.Property<bool>("IsFlagged").HasColumnType("boolean");
                b.Property<string>("RiskLevel").IsRequired().HasColumnType("text");
                b.Property<string>("TriggeredRules").IsRequired().HasColumnType("text");
                b.Property<Guid>("TransactionId").HasColumnType("uuid");
                b.HasKey("Id");
                b.HasIndex("TransactionId").IsUnique();
                b.ToTable("FraudEvaluations");
            });

            modelBuilder.Entity("FraudRuleEngine.Core.Models.Transaction", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uuid");
                b.Property<string>("AccountId").IsRequired().HasColumnType("text");
                b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.Property<string>("Country").IsRequired().HasColumnType("text");
                b.Property<string>("Currency").IsRequired().HasColumnType("text");
                b.Property<string>("MerchantCategory").IsRequired().HasColumnType("text");
                b.Property<string>("MerchantName").IsRequired().HasColumnType("text");
                b.Property<string>("ReferenceNumber").IsRequired().HasColumnType("text");
                b.Property<DateTime>("Timestamp").HasColumnType("timestamp with time zone");
                b.Property<string>("TransactionType").IsRequired().HasColumnType("text");
                b.HasKey("Id");
                b.ToTable("Transactions");
            });

            modelBuilder.Entity("FraudRuleEngine.Core.Models.FraudEvaluation", b =>
            {
                b.HasOne("FraudRuleEngine.Core.Models.Transaction", "Transaction")
                    .WithOne("FraudEvaluation")
                    .HasForeignKey("FraudRuleEngine.Core.Models.FraudEvaluation", "TransactionId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                b.Navigation("Transaction");
            });

            modelBuilder.Entity("FraudRuleEngine.Core.Models.Transaction", b =>
            {
                b.Navigation("FraudEvaluation");
            });
#pragma warning restore 612, 618
        }
    }
}
