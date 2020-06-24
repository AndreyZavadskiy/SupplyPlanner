using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200610_NomenclatureGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "NomenclatureGroup",
                newName: "NomenclatureGroup",
                newSchema: "dic");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "NomenclatureGroup",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dic",
                table: "NomenclatureGroup",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dic",
                table: "NomenclatureGroup");

            migrationBuilder.RenameTable(
                name: "NomenclatureGroup",
                schema: "dic",
                newName: "NomenclatureGroup");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NomenclatureGroup",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
