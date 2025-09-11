using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerLink.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFestivalDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Festivals_FestivalId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Festivals_FestivalId",
                table: "Transactions",
                column: "FestivalId",
                principalTable: "Festivals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Festivals_FestivalId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Festivals_FestivalId",
                table: "Transactions",
                column: "FestivalId",
                principalTable: "Festivals",
                principalColumn: "Id");
        }
    }
}
