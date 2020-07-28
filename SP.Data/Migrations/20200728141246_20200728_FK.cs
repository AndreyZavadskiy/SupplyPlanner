using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200728_FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureBalance_GasStationId",
                table: "NomenclatureBalance",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureBalance_NomenclatureId",
                table: "NomenclatureBalance",
                column: "NomenclatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_NomenclatureBalance_GasStation_GasStationId",
                table: "NomenclatureBalance",
                column: "GasStationId",
                principalTable: "GasStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NomenclatureBalance_Nomenclature_NomenclatureId",
                table: "NomenclatureBalance",
                column: "NomenclatureId",
                principalTable: "Nomenclature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NomenclatureBalance_GasStation_GasStationId",
                table: "NomenclatureBalance");

            migrationBuilder.DropForeignKey(
                name: "FK_NomenclatureBalance_Nomenclature_NomenclatureId",
                table: "NomenclatureBalance");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureBalance_GasStationId",
                table: "NomenclatureBalance");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureBalance_NomenclatureId",
                table: "NomenclatureBalance");
        }
    }
}
