using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200805_NullablePropertiesInGasStation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_CashboxLocation_CashboxLocationId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_ClientRestroom_ClientRestroomId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_TradingHallOperatingMode_TradingHallOperatingModeId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_TradingHallSize_TradingHallSizeId",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "TradingHallSizeId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TradingHallOperatingModeId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ClientRestroomId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CashboxLocationId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_CashboxLocation_CashboxLocationId",
                table: "GasStation",
                column: "CashboxLocationId",
                principalSchema: "dic",
                principalTable: "CashboxLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_ClientRestroom_ClientRestroomId",
                table: "GasStation",
                column: "ClientRestroomId",
                principalSchema: "dic",
                principalTable: "ClientRestroom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_TradingHallOperatingMode_TradingHallOperatingModeId",
                table: "GasStation",
                column: "TradingHallOperatingModeId",
                principalSchema: "dic",
                principalTable: "TradingHallOperatingMode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_TradingHallSize_TradingHallSizeId",
                table: "GasStation",
                column: "TradingHallSizeId",
                principalSchema: "dic",
                principalTable: "TradingHallSize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_CashboxLocation_CashboxLocationId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_ClientRestroom_ClientRestroomId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_TradingHallOperatingMode_TradingHallOperatingModeId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_TradingHallSize_TradingHallSizeId",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "TradingHallSizeId",
                table: "GasStation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TradingHallOperatingModeId",
                table: "GasStation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientRestroomId",
                table: "GasStation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CashboxLocationId",
                table: "GasStation",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_CashboxLocation_CashboxLocationId",
                table: "GasStation",
                column: "CashboxLocationId",
                principalSchema: "dic",
                principalTable: "CashboxLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_ClientRestroom_ClientRestroomId",
                table: "GasStation",
                column: "ClientRestroomId",
                principalSchema: "dic",
                principalTable: "ClientRestroom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_TradingHallOperatingMode_TradingHallOperatingModeId",
                table: "GasStation",
                column: "TradingHallOperatingModeId",
                principalSchema: "dic",
                principalTable: "TradingHallOperatingMode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_TradingHallSize_TradingHallSizeId",
                table: "GasStation",
                column: "TradingHallSizeId",
                principalSchema: "dic",
                principalTable: "TradingHallSize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
