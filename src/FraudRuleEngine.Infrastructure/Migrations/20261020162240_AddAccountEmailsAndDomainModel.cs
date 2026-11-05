using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FraudRuleEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountEmailsAndDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountEmails",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountEmails", x => x.AccountId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountEmails");
        }
    }
}
