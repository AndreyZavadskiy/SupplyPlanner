using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200914_CalcSheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NomCalculation_GasStation_GasStationId",
                table: "NomCalculation");

            migrationBuilder.DropForeignKey(
                name: "FK_NomCalculation_Nomenclature_NomenclatureId",
                table: "NomCalculation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NomCalculation",
                table: "NomCalculation");

            migrationBuilder.RenameTable(
                name: "NomCalculation",
                newName: "CalcSheet");

            migrationBuilder.RenameIndex(
                name: "IX_NomCalculation_NomenclatureId",
                table: "CalcSheet",
                newName: "IX_CalcSheet_NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_NomCalculation_GasStationId",
                table: "CalcSheet",
                newName: "IX_CalcSheet_GasStationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalcSheet",
                table: "CalcSheet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CalcSheet_GasStation_GasStationId",
                table: "CalcSheet",
                column: "GasStationId",
                principalTable: "GasStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalcSheet_Nomenclature_NomenclatureId",
                table: "CalcSheet",
                column: "NomenclatureId",
                principalTable: "Nomenclature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalcSheet_GasStation_GasStationId",
                table: "CalcSheet");

            migrationBuilder.DropForeignKey(
                name: "FK_CalcSheet_Nomenclature_NomenclatureId",
                table: "CalcSheet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalcSheet",
                table: "CalcSheet");

            migrationBuilder.RenameTable(
                name: "CalcSheet",
                newName: "NomCalculation");

            migrationBuilder.RenameIndex(
                name: "IX_CalcSheet_NomenclatureId",
                table: "NomCalculation",
                newName: "IX_NomCalculation_NomenclatureId");

            migrationBuilder.RenameIndex(
                name: "IX_CalcSheet_GasStationId",
                table: "NomCalculation",
                newName: "IX_NomCalculation_GasStationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NomCalculation",
                table: "NomCalculation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NomCalculation_GasStation_GasStationId",
                table: "NomCalculation",
                column: "GasStationId",
                principalTable: "GasStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NomCalculation_Nomenclature_NomenclatureId",
                table: "NomCalculation",
                column: "NomenclatureId",
                principalTable: "Nomenclature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
