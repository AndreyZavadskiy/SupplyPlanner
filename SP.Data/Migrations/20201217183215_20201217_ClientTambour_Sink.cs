using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20201217_ClientTambour_Sink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientSinkTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClientTambourTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientSinkTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "ClientTambourTotal",
                table: "GasStation");
        }
    }
}
