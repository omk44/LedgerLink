using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerLink.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantShopSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create Shops table first
            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShopName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShopEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SubscriptionPlan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubscriptionExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                });

            // Step 2: Insert a default shop for existing data
            migrationBuilder.InsertData(
                table: "Shops",
                columns: new[] { "Id", "ShopName", "ShopEmail", "PhoneNumber", "Address", "SubscriptionPlan", "IsActive", "CreatedAt" },
                values: new object[] { 
                    new Guid("00000000-0000-0000-0000-000000000000"), 
                    "Default Shop", 
                    "admin@ledgerlink.com", 
                    "+919999999999", 
                    "Default Address", 
                    "Free", 
                    true, 
                    DateTime.UtcNow 
                });

            // Step 3: Add ShopId columns to all tables
            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "Payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "Festivals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "DiscountRules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "Customers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShopId",
                table: "Admins",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Step 4: Create indexes on ShopId columns
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ShopId",
                table: "Transactions",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShopId",
                table: "Products",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ShopId",
                table: "Payments",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Festivals_ShopId",
                table: "Festivals",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountRules_ShopId",
                table: "DiscountRules",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ShopId",
                table: "Customers",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_ShopId",
                table: "Admins",
                column: "ShopId");

            // Step 5: Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Shops_ShopId",
                table: "Admins",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Shops_ShopId",
                table: "Customers",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountRules_Shops_ShopId",
                table: "DiscountRules",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Festivals_Shops_ShopId",
                table: "Festivals",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Shops_ShopId",
                table: "Payments",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Shops_ShopId",
                table: "Products",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Shops_ShopId",
                table: "Transactions",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Shops_ShopId",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Shops_ShopId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscountRules_Shops_ShopId",
                table: "DiscountRules");

            migrationBuilder.DropForeignKey(
                name: "FK_Festivals_Shops_ShopId",
                table: "Festivals");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Shops_ShopId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Shops_ShopId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Shops_ShopId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ShopId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Products_ShopId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ShopId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Festivals_ShopId",
                table: "Festivals");

            migrationBuilder.DropIndex(
                name: "IX_DiscountRules_ShopId",
                table: "DiscountRules");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ShopId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Admins_ShopId",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Festivals");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "DiscountRules");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Admins");
        }
    }
}
