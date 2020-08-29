using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200828_DeepFryTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasFrenchFry",
                table: "GasStation");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChequePerDay",
                table: "GasStation",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DeepFryTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeepFryTotal",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "ChequePerDay",
                table: "GasStation",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<bool>(
                name: "HasFrenchFry",
                table: "GasStation",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
