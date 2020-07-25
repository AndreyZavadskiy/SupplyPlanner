using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200725_StageInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                schema: "stage",
                table: "Inventory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MeasureUnitId",
                table: "Inventory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_PersonId",
                schema: "stage",
                table: "Inventory",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_MeasureUnitId",
                table: "Inventory",
                column: "MeasureUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_MeasureUnit_MeasureUnitId",
                table: "Inventory",
                column: "MeasureUnitId",
                principalSchema: "dic",
                principalTable: "MeasureUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Person_PersonId",
                schema: "stage",
                table: "Inventory",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_MeasureUnit_MeasureUnitId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Person_PersonId",
                schema: "stage",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_PersonId",
                schema: "stage",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_MeasureUnitId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "PersonId",
                schema: "stage",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "MeasureUnitId",
                table: "Inventory");
        }
    }
}
