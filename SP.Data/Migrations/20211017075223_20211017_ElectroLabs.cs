using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20211017_ElectroLabs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectricalTestPerYear",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaboratoryWorkSchedule",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecialistTotalForElectricalTest",
                table: "GasStation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectricalTestPerYear",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "LaboratoryWorkSchedule",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "SpecialistTotalForElectricalTest",
                table: "GasStation");
        }
    }
}
