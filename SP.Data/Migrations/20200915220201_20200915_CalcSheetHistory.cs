using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200915_CalcSheetHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalcSheetHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordId = table.Column<int>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: false),
                    NomenclatureId = table.Column<int>(nullable: false),
                    GasStationId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    FixedAmount = table.Column<decimal>(type: "decimal(19,4)", nullable: true),
                    Formula = table.Column<string>(nullable: true),
                    MultipleFactor = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    Rounding = table.Column<int>(nullable: false),
                    Plan = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalcSheetHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalcSheetHistory");
        }
    }
}
