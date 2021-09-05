using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20210905_NewObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_ManagementSystem_ManagementSystemId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_OperatorRoomFormat_OperatorRoomFormatId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_ServiceLevel_ServiceLevelId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_StationLocation_StationLocationId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_StationStatus_StationStatusId",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "StationStatusId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "StationLocationId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "SettlementId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceLevelId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "SegmentId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "RevenueAvg",
                table: "GasStation",
                type: "decimal(12,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RepresentativenessFactor3Quarter",
                table: "GasStation",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RepresentativenessFactor",
                table: "GasStation",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PersonnelPerDay",
                table: "GasStation",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");

            migrationBuilder.AlterColumn<int>(
                name: "OperatorRoomFormatId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "NightRefuelingTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "NightCleaningTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "MerrychefTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerArmTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ManagementSystemId",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FuelDispenserTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FuelDispenserPostWithoutShedTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FuelDispenserPostTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DishWashingMachineTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DeepFryTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DayRefuelingTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DayCleaningTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CoffeeMachineTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CodeKSSS",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ClientTambourTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ClientSinkTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ClientRestroomTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChequePerDay",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChequeBandLengthPerDay",
                table: "GasStation",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");

            migrationBuilder.AlterColumn<int>(
                name: "CashboxTotal",
                table: "GasStation",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "AntiIcingPerYear",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AntiIcingSquare",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AverageTestPerMonth",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DieselFuelPerYear",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiningRoomTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlagpoleTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Fuel100PerYear",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Fuel92PerYear",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Fuel95PerYear",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FuelTrackPerYear",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasCentralWaterSupply",
                table: "GasStation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasFuelBaseAutomation",
                table: "GasStation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSindyAnalyzer",
                table: "GasStation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSpectroscan",
                table: "GasStation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HasWell",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonnelPerShift",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonnelTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RailwayDeliveryPlanTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RailwayTankPerYear",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservoirTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RestroomTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServicingGasStationTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShiftPerDay",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StampTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingPlaceTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingRoomTotal",
                table: "GasStation",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_ManagementSystem_ManagementSystemId",
                table: "GasStation",
                column: "ManagementSystemId",
                principalSchema: "dic",
                principalTable: "ManagementSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_OperatorRoomFormat_OperatorRoomFormatId",
                table: "GasStation",
                column: "OperatorRoomFormatId",
                principalSchema: "dic",
                principalTable: "OperatorRoomFormat",
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

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_ServiceLevel_ServiceLevelId",
                table: "GasStation",
                column: "ServiceLevelId",
                principalSchema: "dic",
                principalTable: "ServiceLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_StationLocation_StationLocationId",
                table: "GasStation",
                column: "StationLocationId",
                principalSchema: "dic",
                principalTable: "StationLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_StationStatus_StationStatusId",
                table: "GasStation",
                column: "StationStatusId",
                principalSchema: "dic",
                principalTable: "StationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_ManagementSystem_ManagementSystemId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_OperatorRoomFormat_OperatorRoomFormatId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_ServiceLevel_ServiceLevelId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_StationLocation_StationLocationId",
                table: "GasStation");

            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_StationStatus_StationStatusId",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "AntiIcingPerYear",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "AntiIcingSquare",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "AverageTestPerMonth",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "DepartmentTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "DieselFuelPerYear",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "DiningRoomTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "FlagpoleTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "Fuel100PerYear",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "Fuel92PerYear",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "Fuel95PerYear",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "FuelTrackPerYear",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "HasCentralWaterSupply",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "HasFuelBaseAutomation",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "HasSindyAnalyzer",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "HasSpectroscan",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "HasWell",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "PersonnelPerShift",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "PersonnelTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "RailwayDeliveryPlanTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "RailwayTankPerYear",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "ReservoirTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "RestroomTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "ServicingGasStationTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "ShiftPerDay",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "StampTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "WorkingPlaceTotal",
                table: "GasStation");

            migrationBuilder.DropColumn(
                name: "WorkingRoomTotal",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "StationStatusId",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StationLocationId",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SettlementId",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceLevelId",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SegmentId",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RevenueAvg",
                table: "GasStation",
                type: "decimal(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RepresentativenessFactor3Quarter",
                table: "GasStation",
                type: "decimal(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RepresentativenessFactor",
                table: "GasStation",
                type: "decimal(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PersonnelPerDay",
                table: "GasStation",
                type: "decimal(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OperatorRoomFormatId",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NightRefuelingTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NightCleaningTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MerrychefTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManagerArmTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManagementSystemId",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FuelDispenserTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FuelDispenserPostWithoutShedTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FuelDispenserPostTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DishWashingMachineTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeepFryTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DayRefuelingTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DayCleaningTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CoffeeMachineTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CodeKSSS",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientTambourTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientSinkTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientRestroomTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ChequePerDay",
                table: "GasStation",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ChequeBandLengthPerDay",
                table: "GasStation",
                type: "decimal(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CashboxTotal",
                table: "GasStation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

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
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation",
                column: "SegmentId",
                principalSchema: "dic",
                principalTable: "Segment",
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
        }
    }
}
