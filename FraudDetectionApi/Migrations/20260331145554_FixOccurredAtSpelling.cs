using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FraudDetectionApi.Migrations
{
    /// <inheritdoc />
    public partial class FixOccurredAtSpelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccuredAt",
                table: "Transactions",
                newName: "OccurredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "Transactions",
                newName: "OccuredAt");
        }
    }
}
