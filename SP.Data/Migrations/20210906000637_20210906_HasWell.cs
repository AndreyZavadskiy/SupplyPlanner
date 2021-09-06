using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20210906_HasWell : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasWell",
                table: "GasStation");

            migrationBuilder.AddColumn<bool>(
                name: "HasWell",
                table: "GasStation",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HasWell",
                table: "GasStation",
                type: "integer",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
