using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200728_NomenclatureBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Nomenclature",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "NomenclatureBalance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomenclatureId = table.Column<int>(nullable: false),
                    GasStationId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureBalance", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NomenclatureBalance");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Nomenclature");
        }
    }
}
