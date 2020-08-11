using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200808_NomCalculation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NomenclatureBalance");

            migrationBuilder.DropTable(
                name: "Requirement");

            migrationBuilder.CreateTable(
                name: "NomCalculation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_NomCalculation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomCalculation_GasStation_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NomCalculation_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NomCalculation_GasStationId",
                table: "NomCalculation",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_NomCalculation_NomenclatureId",
                table: "NomCalculation",
                column: "NomenclatureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NomCalculation");

            migrationBuilder.CreateTable(
                name: "NomenclatureBalance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GasStationId = table.Column<int>(type: "int", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NomenclatureId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NomenclatureBalance_GasStation_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NomenclatureBalance_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requirement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FixedAmount = table.Column<decimal>(type: "decimal(19,4)", nullable: true),
                    Formula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GasStationId = table.Column<int>(type: "int", nullable: false),
                    MultipleFactor = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    NomenclatureId = table.Column<int>(type: "int", nullable: false),
                    Plan = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    Rounding = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requirement_GasStation_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requirement_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureBalance_GasStationId",
                table: "NomenclatureBalance",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureBalance_NomenclatureId",
                table: "NomenclatureBalance",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirement_GasStationId",
                table: "Requirement",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirement_NomenclatureId",
                table: "Requirement",
                column: "NomenclatureId");
        }
    }
}
