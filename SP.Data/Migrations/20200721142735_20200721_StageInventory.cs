using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200721_StageInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_CashboxLocation_CashboxLocationId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_ClientRestroom_ClientRestroomId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_ManagementSystem_ManagementSystemId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_OperatorRoomFormat_OperatorRoomFormatId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_ServiceLevel_ServiceLevelId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_Settlement_SettlememtId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_StationLocation_StationLocationId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_StationStatus_StationStatusId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_RegionalStructure_TerritoryId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_TradingHallOperatingMode_TradingHallOperatingModeId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStations_TradingHallSize_TradingHallSizeId",
                table: "GasStations");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_GasStations_GasStationId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirement_GasStations_GasStationId",
                table: "Requirement");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_GasStations_GasStationId",
                schema: "stage",
                table: "Inventory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GasStations",
                table: "GasStations");

            migrationBuilder.RenameTable(
                name: "GasStations",
                newName: "GasStation");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_TradingHallSizeId",
                table: "GasStation",
                newName: "IX_GasStation_TradingHallSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_TradingHallOperatingModeId",
                table: "GasStation",
                newName: "IX_GasStation_TradingHallOperatingModeId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_TerritoryId",
                table: "GasStation",
                newName: "IX_GasStation_TerritoryId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_StationStatusId",
                table: "GasStation",
                newName: "IX_GasStation_StationStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_StationLocationId",
                table: "GasStation",
                newName: "IX_GasStation_StationLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_SettlememtId",
                table: "GasStation",
                newName: "IX_GasStation_SettlememtId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_ServiceLevelId",
                table: "GasStation",
                newName: "IX_GasStation_ServiceLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_OperatorRoomFormatId",
                table: "GasStation",
                newName: "IX_GasStation_OperatorRoomFormatId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_ManagementSystemId",
                table: "GasStation",
                newName: "IX_GasStation_ManagementSystemId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_ClientRestroomId",
                table: "GasStation",
                newName: "IX_GasStation_ClientRestroomId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStations_CashboxLocationId",
                table: "GasStation",
                newName: "IX_GasStation_CashboxLocationId");

            migrationBuilder.AddColumn<int>(
                name: "MeasureUnitId",
                schema: "stage",
                table: "Inventory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "TradingHallSize",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "TradingHallOperatingMode",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "StationStatus",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "StationLocation",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "Settlement",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "ServiceLevel",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "OperatorRoomFormat",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "NomenclatureGroup",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "MeasureUnit",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "ManagementSystem",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "ClientRestroom",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "CashboxLocation",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GasStation",
                table: "GasStation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_MeasureUnitId",
                schema: "stage",
                table: "Inventory",
                column: "MeasureUnitId");

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
                name: "FK_GasStation_ManagementSystem_ManagementSystemId",
                table: "GasStation",
                column: "ManagementSystemId",
                principalSchema: "dic",
                principalTable: "ManagementSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_OperatorRoomFormat_OperatorRoomFormatId",
                table: "GasStation",
                column: "OperatorRoomFormatId",
                principalSchema: "dic",
                principalTable: "OperatorRoomFormat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_ServiceLevel_ServiceLevelId",
                table: "GasStation",
                column: "ServiceLevelId",
                principalSchema: "dic",
                principalTable: "ServiceLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_Settlement_SettlememtId",
                table: "GasStation",
                column: "SettlememtId",
                principalSchema: "dic",
                principalTable: "Settlement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_StationLocation_StationLocationId",
                table: "GasStation",
                column: "StationLocationId",
                principalSchema: "dic",
                principalTable: "StationLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_StationStatus_StationStatusId",
                table: "GasStation",
                column: "StationStatusId",
                principalSchema: "dic",
                principalTable: "StationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_RegionalStructure_TerritoryId",
                table: "GasStation",
                column: "TerritoryId",
                principalTable: "RegionalStructure",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_GasStation_GasStationId",
                table: "Inventory",
                column: "GasStationId",
                principalTable: "GasStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requirement_GasStation_GasStationId",
                table: "Requirement",
                column: "GasStationId",
                principalTable: "GasStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_GasStation_GasStationId",
                schema: "stage",
                table: "Inventory",
                column: "GasStationId",
                principalTable: "GasStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_MeasureUnit_MeasureUnitId",
                schema: "stage",
                table: "Inventory",
                column: "MeasureUnitId",
                principalSchema: "dic",
                principalTable: "MeasureUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
                name: "FK_GasStation_ManagementSystem_ManagementSystemId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_OperatorRoomFormat_OperatorRoomFormatId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_ServiceLevel_ServiceLevelId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_Settlement_SettlememtId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_StationLocation_StationLocationId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_StationStatus_StationStatusId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_RegionalStructure_TerritoryId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_TradingHallOperatingMode_TradingHallOperatingModeId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_TradingHallSize_TradingHallSizeId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_GasStation_GasStationId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirement_GasStation_GasStationId",
                table: "Requirement");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_GasStation_GasStationId",
                schema: "stage",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_MeasureUnit_MeasureUnitId",
                schema: "stage",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_MeasureUnitId",
                schema: "stage",
                table: "Inventory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GasStation",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "MeasureUnitId",
                schema: "stage",
                table: "Inventory");

            migrationBuilder.RenameTable(
                name: "GasStation",
                newName: "GasStations");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_TradingHallSizeId",
                table: "GasStations",
                newName: "IX_GasStations_TradingHallSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_TradingHallOperatingModeId",
                table: "GasStations",
                newName: "IX_GasStations_TradingHallOperatingModeId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_TerritoryId",
                table: "GasStations",
                newName: "IX_GasStations_TerritoryId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_StationStatusId",
                table: "GasStations",
                newName: "IX_GasStations_StationStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_StationLocationId",
                table: "GasStations",
                newName: "IX_GasStations_StationLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_SettlememtId",
                table: "GasStations",
                newName: "IX_GasStations_SettlememtId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_ServiceLevelId",
                table: "GasStations",
                newName: "IX_GasStations_ServiceLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_OperatorRoomFormatId",
                table: "GasStations",
                newName: "IX_GasStations_OperatorRoomFormatId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_ManagementSystemId",
                table: "GasStations",
                newName: "IX_GasStations_ManagementSystemId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_ClientRestroomId",
                table: "GasStations",
                newName: "IX_GasStations_ClientRestroomId");

            migrationBuilder.RenameIndex(
                name: "IX_GasStation_CashboxLocationId",
                table: "GasStations",
                newName: "IX_GasStations_CashboxLocationId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "TradingHallSize",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "TradingHallOperatingMode",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "StationStatus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "StationLocation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "Settlement",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "ServiceLevel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "OperatorRoomFormat",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "NomenclatureGroup",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "MeasureUnit",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "ManagementSystem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "ClientRestroom",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dic",
                table: "CashboxLocation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GasStations",
                table: "GasStations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_CashboxLocation_CashboxLocationId",
                table: "GasStations",
                column: "CashboxLocationId",
                principalSchema: "dic",
                principalTable: "CashboxLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_ClientRestroom_ClientRestroomId",
                table: "GasStations",
                column: "ClientRestroomId",
                principalSchema: "dic",
                principalTable: "ClientRestroom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_ManagementSystem_ManagementSystemId",
                table: "GasStations",
                column: "ManagementSystemId",
                principalSchema: "dic",
                principalTable: "ManagementSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_OperatorRoomFormat_OperatorRoomFormatId",
                table: "GasStations",
                column: "OperatorRoomFormatId",
                principalSchema: "dic",
                principalTable: "OperatorRoomFormat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_ServiceLevel_ServiceLevelId",
                table: "GasStations",
                column: "ServiceLevelId",
                principalSchema: "dic",
                principalTable: "ServiceLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_Settlement_SettlememtId",
                table: "GasStations",
                column: "SettlememtId",
                principalSchema: "dic",
                principalTable: "Settlement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_StationLocation_StationLocationId",
                table: "GasStations",
                column: "StationLocationId",
                principalSchema: "dic",
                principalTable: "StationLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_StationStatus_StationStatusId",
                table: "GasStations",
                column: "StationStatusId",
                principalSchema: "dic",
                principalTable: "StationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_RegionalStructure_TerritoryId",
                table: "GasStations",
                column: "TerritoryId",
                principalTable: "RegionalStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_TradingHallOperatingMode_TradingHallOperatingModeId",
                table: "GasStations",
                column: "TradingHallOperatingModeId",
                principalSchema: "dic",
                principalTable: "TradingHallOperatingMode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStations_TradingHallSize_TradingHallSizeId",
                table: "GasStations",
                column: "TradingHallSizeId",
                principalSchema: "dic",
                principalTable: "TradingHallSize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_GasStations_GasStationId",
                table: "Inventory",
                column: "GasStationId",
                principalTable: "GasStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requirement_GasStations_GasStationId",
                table: "Requirement",
                column: "GasStationId",
                principalTable: "GasStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_GasStations_GasStationId",
                schema: "stage",
                table: "Inventory",
                column: "GasStationId",
                principalTable: "GasStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
