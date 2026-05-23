using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Net.Kidd.Habitizer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "portfolio");

            migrationBuilder.CreateTable(
                name: "Instruments",
                schema: "portfolio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Isin = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                schema: "portfolio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "portfolio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastUpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Sources_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "portfolio",
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                schema: "portfolio",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetSize = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnrealisedAverageCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnrealisedProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnrealisedProfitPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReferencePrice_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReferencePrice_Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferencePrice_Exchange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferencePrice_CurrencySpot = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => new { x.AccountId, x.InstrumentId });
                    table.ForeignKey(
                        name: "FK_Positions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "portfolio",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Positions_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalSchema: "portfolio",
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_SourceId_Name",
                schema: "portfolio",
                table: "Accounts",
                columns: new[] { "SourceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Isin",
                schema: "portfolio",
                table: "Instruments",
                column: "Isin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_InstrumentId",
                schema: "portfolio",
                table: "Positions",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Name",
                schema: "portfolio",
                table: "Sources",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Positions",
                schema: "portfolio");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "portfolio");

            migrationBuilder.DropTable(
                name: "Instruments",
                schema: "portfolio");

            migrationBuilder.DropTable(
                name: "Sources",
                schema: "portfolio");
        }
    }
}
