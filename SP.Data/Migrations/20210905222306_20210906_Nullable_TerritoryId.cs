using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20210906_Nullable_TerritoryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_RegionalStructure_TerritoryId",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "TerritoryId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_RegionalStructure_TerritoryId",
                table: "GasStation",
                column: "TerritoryId",
                principalTable: "RegionalStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_RegionalStructure_TerritoryId",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "TerritoryId",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_RegionalStructure_TerritoryId",
                table: "GasStation",
                column: "TerritoryId",
                principalTable: "RegionalStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
