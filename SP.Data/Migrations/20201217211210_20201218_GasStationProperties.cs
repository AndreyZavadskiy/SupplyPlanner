using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20201218_GasStationProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayCleaningTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuelDispenserPostTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NightCleaningTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayCleaningTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "FuelDispenserPostTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "NightCleaningTotal",
                table: "GasStation");
        }
    }
}
