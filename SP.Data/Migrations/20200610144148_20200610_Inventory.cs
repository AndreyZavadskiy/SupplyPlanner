using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200610_Inventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "stage");

            migrationBuilder.CreateTable(
                name: "NomenclatureGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasureUnit",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                schema: "stage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    GasStationId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_GasStations_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclature",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    PetronicsCode = table.Column<string>(maxLength: 20, nullable: true),
                    PetronicsName = table.Column<string>(maxLength: 100, nullable: true),
                    MeasureUnitId = table.Column<int>(nullable: false),
                    NomenclatureGroupId = table.Column<int>(nullable: false),
                    UsefulLife = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nomenclature_MeasureUnit_MeasureUnitId",
                        column: x => x.MeasureUnitId,
                        principalSchema: "dic",
                        principalTable: "MeasureUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nomenclature_NomenclatureGroup_NomenclatureGroupId",
                        column: x => x.NomenclatureGroupId,
                        principalTable: "NomenclatureGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    NomenclatureId = table.Column<int>(nullable: true),
                    GasStationId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    IsBlocked = table.Column<bool>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_GasStations_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requirement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomenclatureId = table.Column<int>(nullable: false),
                    GasStationId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    Formula = table.Column<string>(nullable: true),
                    MultipleFactor = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    Rounding = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requirement_GasStations_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStations",
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
                name: "IX_Inventory_GasStationId",
                table: "Inventory",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_NomenclatureId",
                table: "Inventory",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclature_MeasureUnitId",
                table: "Nomenclature",
                column: "MeasureUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclature_NomenclatureGroupId",
                table: "Nomenclature",
                column: "NomenclatureGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirement_GasStationId",
                table: "Requirement",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirement_NomenclatureId",
                table: "Requirement",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_GasStationId",
                schema: "stage",
                table: "Inventory",
                column: "GasStationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Requirement");

            migrationBuilder.DropTable(
                name: "Inventory",
                schema: "stage");

            migrationBuilder.DropTable(
                name: "Nomenclature");

            migrationBuilder.DropTable(
                name: "MeasureUnit",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "NomenclatureGroup");
        }
    }
}
