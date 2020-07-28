using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200729_Requirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Requirement",
                newName: "Plan");

            migrationBuilder.AddColumn<decimal>(
                name: "FixedAmount",
                table: "Requirement",
                type: "decimal(19,4)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedAmount",
                table: "Requirement");

            migrationBuilder.RenameColumn(
                name: "Plan",
                table: "Requirement",
                newName: "Quantity");
        }
    }
}
