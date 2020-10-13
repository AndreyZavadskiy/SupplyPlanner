using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20201013_UniqueIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventory_GasStationId",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_CalcSheet_GasStationId",
                table: "CalcSheet");

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                schema: "log",
                table: "Change",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Inventory",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionalStructure_ParentId",
                table: "RegionalStructure",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_GasStationId_Code",
                table: "Inventory",
                columns: new[] { "GasStationId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CalcSheet_GasStationId_NomenclatureId",
                table: "CalcSheet",
                columns: new[] { "GasStationId", "NomenclatureId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RegionalStructure_RegionalStructure_ParentId",
                table: "RegionalStructure",
                column: "ParentId",
                principalTable: "RegionalStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegionalStructure_RegionalStructure_ParentId",
                table: "RegionalStructure");

            migrationBuilder.DropIndex(
                name: "IX_RegionalStructure_ParentId",
                table: "RegionalStructure");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_GasStationId_Code",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_CalcSheet_GasStationId_NomenclatureId",
                table: "CalcSheet");

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                schema: "log",
                table: "Change",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Inventory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_GasStationId",
                table: "Inventory",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_CalcSheet_GasStationId",
                table: "CalcSheet",
                column: "GasStationId");
        }
    }
}
