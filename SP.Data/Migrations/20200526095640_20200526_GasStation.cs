using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200526_GasStation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settlement",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GasStations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeKSSS = table.Column<int>(nullable: false),
                    CodeSAP = table.Column<string>(maxLength: 20, nullable: true),
                    StationNumber = table.Column<string>(maxLength: 5, nullable: true),
                    TerritoryId = table.Column<int>(nullable: false),
                    SettlementId = table.Column<int>(nullable: false),
                    Address = table.Column<string>(maxLength: 200, nullable: true),
                    StationLocationId = table.Column<int>(nullable: false),
                    StationStatusId = table.Column<int>(nullable: false),
                    ServiceLevelId = table.Column<int>(nullable: false),
                    OperatorRoomFormatId = table.Column<int>(nullable: false),
                    ManagementSystemId = table.Column<int>(nullable: false),
                    TradingHallOperatingModeId = table.Column<int>(nullable: false),
                    ClientRestroomId = table.Column<int>(nullable: false),
                    CashboxLocationId = table.Column<int>(nullable: false),
                    TradingHallSizeId = table.Column<int>(nullable: false),
                    CashboxTotal = table.Column<int>(nullable: false),
                    PersonnelPerDay = table.Column<int>(nullable: false),
                    FuelDispenserTotal = table.Column<int>(nullable: false),
                    ClientRestroomTotal = table.Column<int>(nullable: false),
                    TradingHallArea = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    ChequePerDay = table.Column<int>(nullable: false),
                    RevenueAvg = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    HasSibilla = table.Column<bool>(nullable: false),
                    HasBakery = table.Column<bool>(nullable: false),
                    HasCakes = table.Column<bool>(nullable: false),
                    HasFrenchFry = table.Column<bool>(nullable: false),
                    HasMarmite = table.Column<bool>(nullable: false),
                    HasKitchen = table.Column<bool>(nullable: false),
                    CoffeeMachineTotal = table.Column<int>(nullable: false),
                    DishWashingMachineTotal = table.Column<int>(nullable: false),
                    SettlememtId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GasStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GasStations_CashboxLocation_CashboxLocationId",
                        column: x => x.CashboxLocationId,
                        principalSchema: "dic",
                        principalTable: "CashboxLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_ClientRestroom_ClientRestroomId",
                        column: x => x.ClientRestroomId,
                        principalSchema: "dic",
                        principalTable: "ClientRestroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_ManagementSystem_ManagementSystemId",
                        column: x => x.ManagementSystemId,
                        principalSchema: "dic",
                        principalTable: "ManagementSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_OperatorRoomFormat_OperatorRoomFormatId",
                        column: x => x.OperatorRoomFormatId,
                        principalSchema: "dic",
                        principalTable: "OperatorRoomFormat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_ServiceLevel_ServiceLevelId",
                        column: x => x.ServiceLevelId,
                        principalSchema: "dic",
                        principalTable: "ServiceLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_Settlement_SettlememtId",
                        column: x => x.SettlememtId,
                        principalSchema: "dic",
                        principalTable: "Settlement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GasStations_StationLocation_StationLocationId",
                        column: x => x.StationLocationId,
                        principalSchema: "dic",
                        principalTable: "StationLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_StationStatus_StationStatusId",
                        column: x => x.StationStatusId,
                        principalSchema: "dic",
                        principalTable: "StationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_RegionalStructure_TerritoryId",
                        column: x => x.TerritoryId,
                        principalTable: "RegionalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_TradingHallOperatingMode_TradingHallOperatingModeId",
                        column: x => x.TradingHallOperatingModeId,
                        principalSchema: "dic",
                        principalTable: "TradingHallOperatingMode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStations_TradingHallSize_TradingHallSizeId",
                        column: x => x.TradingHallSizeId,
                        principalSchema: "dic",
                        principalTable: "TradingHallSize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_CashboxLocationId",
                table: "GasStations",
                column: "CashboxLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_ClientRestroomId",
                table: "GasStations",
                column: "ClientRestroomId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_ManagementSystemId",
                table: "GasStations",
                column: "ManagementSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_OperatorRoomFormatId",
                table: "GasStations",
                column: "OperatorRoomFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_ServiceLevelId",
                table: "GasStations",
                column: "ServiceLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_SettlememtId",
                table: "GasStations",
                column: "SettlememtId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_StationLocationId",
                table: "GasStations",
                column: "StationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_StationStatusId",
                table: "GasStations",
                column: "StationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_TerritoryId",
                table: "GasStations",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_TradingHallOperatingModeId",
                table: "GasStations",
                column: "TradingHallOperatingModeId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStations_TradingHallSizeId",
                table: "GasStations",
                column: "TradingHallSizeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GasStations");

            migrationBuilder.DropTable(
                name: "Settlement",
                schema: "dic");
        }
    }
}
