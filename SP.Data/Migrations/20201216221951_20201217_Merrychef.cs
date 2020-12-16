using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20201217_Merrychef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MerrychefTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerrychefTotal",
                table: "GasStation");
        }
    }
}
