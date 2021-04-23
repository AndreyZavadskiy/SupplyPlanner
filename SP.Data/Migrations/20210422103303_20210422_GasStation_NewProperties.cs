using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SP.Data.Migrations
{
    public partial class _20210422_GasStation_NewProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CashRegisterTapeId",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayRefuelingTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuelDispenserPostWithoutShedTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasFuelCardProgram",
                table: "GasStation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ManagerArmTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NightRefuelingTotal",
                table: "GasStation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SegmentId",
                table: "GasStation",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CashRegisterTape",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRegisterTape", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Segment",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_CashRegisterTapeId",
                table: "GasStation",
                column: "CashRegisterTapeId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_SegmentId",
                table: "GasStation",
                column: "SegmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_CashRegisterTape_CashRegisterTapeId",
                table: "GasStation",
                column: "CashRegisterTapeId",
                principalSchema: "dic",
                principalTable: "CashRegisterTape",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation",
                column: "SegmentId",
                principalSchema: "dic",
                principalTable: "Segment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_CashRegisterTape_CashRegisterTapeId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation");

            migrationBuilder.DropTable(
                name: "CashRegisterTape",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "Segment",
                schema: "dic");

            migrationBuilder.DropIndex(
                name: "IX_GasStation_CashRegisterTapeId",
                table: "GasStation");

            migrationBuilder.DropIndex(
                name: "IX_GasStation_SegmentId",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "CashRegisterTapeId",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "DayRefuelingTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "FuelDispenserPostWithoutShedTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "HasFuelCardProgram",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "ManagerArmTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "NightRefuelingTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "SegmentId",
                table: "GasStation");
        }
    }
}
