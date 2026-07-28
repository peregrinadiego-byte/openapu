using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenAPU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "apus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    UnitCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UnitSymbol = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    UnitName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "concepts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    UnitCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UnitSymbol = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    UnitName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ApuId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IndirectCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    Financing = table.Column<decimal>(type: "TEXT", nullable: false),
                    Profit = table.Column<decimal>(type: "TEXT", nullable: false),
                    AdditionalCharges = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_concepts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UnitCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UnitSymbol = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    UnitName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "apu_components",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ApuId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apu_components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_apu_components_apus_ApuId",
                        column: x => x.ApuId,
                        principalTable: "apus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "budget_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BudgetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConceptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_budget_items_budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_apu_components_ApuId_ResourceId",
                table: "apu_components",
                columns: new[] { "ApuId", "ResourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_apus_Key",
                table: "apus",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budget_items_BudgetId_ConceptId",
                table: "budget_items",
                columns: new[] { "BudgetId", "ConceptId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_Key",
                table: "budgets",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_concepts_Key",
                table: "concepts",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resources_Key",
                table: "resources",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apu_components");

            migrationBuilder.DropTable(
                name: "budget_items");

            migrationBuilder.DropTable(
                name: "concepts");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "apus");

            migrationBuilder.DropTable(
                name: "budgets");
        }
    }
}
