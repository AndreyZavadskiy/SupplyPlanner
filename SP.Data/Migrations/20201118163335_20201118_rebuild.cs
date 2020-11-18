using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20201118_rebuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "log");

            migrationBuilder.EnsureSchema(
                name: "dic");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    FriendlyName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionalStructure",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalStructure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionalStructure_RegionalStructure_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RegionalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashboxLocation",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashboxLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientRestroom",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRestroom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManagementSystem",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasureUnit",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NomenclatureGroup",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NomenclatureGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatorRoomFormat",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorRoomFormat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLevel",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settlement",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationLocation",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationStatus",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradingHallOperatingMode",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingHallOperatingMode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradingHallSize",
                schema: "dic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingHallSize", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    AspNetUserId = table.Column<string>(maxLength: 450, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclature",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    PetronicsCode = table.Column<string>(maxLength: 20, nullable: true),
                    PetronicsName = table.Column<string>(maxLength: 100, nullable: true),
                    MeasureUnitId = table.Column<int>(nullable: false),
                    NomenclatureGroupId = table.Column<int>(nullable: false),
                    UsefulLife = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nomenclature_MeasureUnit_MeasureUnitId",
                        column: x => x.MeasureUnitId,
                        principalSchema: "dic",
                        principalTable: "MeasureUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nomenclature_NomenclatureGroup_NomenclatureGroupId",
                        column: x => x.NomenclatureGroupId,
                        principalSchema: "dic",
                        principalTable: "NomenclatureGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GasStation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
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
                    TradingHallOperatingModeId = table.Column<int>(nullable: true),
                    ClientRestroomId = table.Column<int>(nullable: true),
                    CashboxLocationId = table.Column<int>(nullable: true),
                    TradingHallSizeId = table.Column<int>(nullable: true),
                    CashboxTotal = table.Column<int>(nullable: false),
                    PersonnelPerDay = table.Column<int>(nullable: false),
                    FuelDispenserTotal = table.Column<int>(nullable: false),
                    ClientRestroomTotal = table.Column<int>(nullable: false),
                    TradingHallArea = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    ChequePerDay = table.Column<decimal>(nullable: false),
                    RevenueAvg = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    HasJointRestroomEntrance = table.Column<bool>(nullable: false),
                    HasSibilla = table.Column<bool>(nullable: false),
                    HasBakery = table.Column<bool>(nullable: false),
                    HasCakes = table.Column<bool>(nullable: false),
                    DeepFryTotal = table.Column<int>(nullable: false),
                    HasMarmite = table.Column<bool>(nullable: false),
                    HasKitchen = table.Column<bool>(nullable: false),
                    CoffeeMachineTotal = table.Column<int>(nullable: false),
                    DishWashingMachineTotal = table.Column<int>(nullable: false),
                    ChequeBandLengthPerDay = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    RepresentativenessFactor = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    SettlememtId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GasStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GasStation_CashboxLocation_CashboxLocationId",
                        column: x => x.CashboxLocationId,
                        principalSchema: "dic",
                        principalTable: "CashboxLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GasStation_ClientRestroom_ClientRestroomId",
                        column: x => x.ClientRestroomId,
                        principalSchema: "dic",
                        principalTable: "ClientRestroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GasStation_ManagementSystem_ManagementSystemId",
                        column: x => x.ManagementSystemId,
                        principalSchema: "dic",
                        principalTable: "ManagementSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStation_OperatorRoomFormat_OperatorRoomFormatId",
                        column: x => x.OperatorRoomFormatId,
                        principalSchema: "dic",
                        principalTable: "OperatorRoomFormat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStation_ServiceLevel_ServiceLevelId",
                        column: x => x.ServiceLevelId,
                        principalSchema: "dic",
                        principalTable: "ServiceLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStation_Settlement_SettlememtId",
                        column: x => x.SettlememtId,
                        principalSchema: "dic",
                        principalTable: "Settlement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GasStation_StationLocation_StationLocationId",
                        column: x => x.StationLocationId,
                        principalSchema: "dic",
                        principalTable: "StationLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStation_StationStatus_StationStatusId",
                        column: x => x.StationStatusId,
                        principalSchema: "dic",
                        principalTable: "StationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStation_RegionalStructure_TerritoryId",
                        column: x => x.TerritoryId,
                        principalTable: "RegionalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GasStation_TradingHallOperatingMode_TradingHallOperatingModeId",
                        column: x => x.TradingHallOperatingModeId,
                        principalSchema: "dic",
                        principalTable: "TradingHallOperatingMode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GasStation_TradingHallSize_TradingHallSizeId",
                        column: x => x.TradingHallSizeId,
                        principalSchema: "dic",
                        principalTable: "TradingHallSize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    OrderType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonTerritory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(nullable: false),
                    RegionalStructureId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonTerritory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonTerritory_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonTerritory_RegionalStructure_RegionalStructureId",
                        column: x => x.RegionalStructureId,
                        principalTable: "RegionalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Action",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(nullable: false),
                    ActionDate = table.Column<DateTime>(nullable: false),
                    Category = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Action_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Change",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(nullable: false),
                    ChangeDate = table.Column<DateTime>(nullable: false),
                    EntityName = table.Column<string>(maxLength: 50, nullable: true),
                    RecordId = table.Column<int>(nullable: false),
                    OldValue = table.Column<string>(nullable: true),
                    NewValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Change", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Change_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalcSheet",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomenclatureId = table.Column<int>(nullable: false),
                    GasStationId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    FixedAmount = table.Column<decimal>(type: "decimal(19,4)", nullable: true),
                    Formula = table.Column<string>(nullable: true),
                    MultipleFactor = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    Rounding = table.Column<int>(nullable: false),
                    Plan = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalcSheet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalcSheet_GasStation_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalcSheet_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalcSheetHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordId = table.Column<long>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: false),
                    NomenclatureId = table.Column<int>(nullable: false),
                    GasStationId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    FixedAmount = table.Column<decimal>(type: "decimal(19,4)", nullable: true),
                    Formula = table.Column<string>(nullable: true),
                    MultipleFactor = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    Rounding = table.Column<int>(nullable: false),
                    Plan = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalcSheetHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalcSheetHistory_GasStation_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalcSheetHistory_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    GasStationId = table.Column<int>(nullable: false),
                    MeasureUnitId = table.Column<int>(nullable: false),
                    NomenclatureId = table.Column<int>(nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    IsBlocked = table.Column<bool>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_GasStation_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_MeasureUnit_MeasureUnitId",
                        column: x => x.MeasureUnitId,
                        principalSchema: "dic",
                        principalTable: "MeasureUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StageInventory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    GasStationId = table.Column<int>(nullable: false),
                    MeasureUnitId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageInventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageInventory_GasStation_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StageInventory_MeasureUnit_MeasureUnitId",
                        column: x => x.MeasureUnitId,
                        principalSchema: "dic",
                        principalTable: "MeasureUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StageInventory_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(nullable: false),
                    NomenclatureId = table.Column<int>(nullable: false),
                    GasStationId = table.Column<int>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_GasStation_GasStationId",
                        column: x => x.GasStationId,
                        principalTable: "GasStation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Nomenclature_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CalcSheet_NomenclatureId",
                table: "CalcSheet",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_CalcSheet_GasStationId_NomenclatureId",
                table: "CalcSheet",
                columns: new[] { "GasStationId", "NomenclatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalcSheetHistory_GasStationId",
                table: "CalcSheetHistory",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_CalcSheetHistory_NomenclatureId",
                table: "CalcSheetHistory",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_CashboxLocationId",
                table: "GasStation",
                column: "CashboxLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_ClientRestroomId",
                table: "GasStation",
                column: "ClientRestroomId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_ManagementSystemId",
                table: "GasStation",
                column: "ManagementSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_OperatorRoomFormatId",
                table: "GasStation",
                column: "OperatorRoomFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_ServiceLevelId",
                table: "GasStation",
                column: "ServiceLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_SettlememtId",
                table: "GasStation",
                column: "SettlememtId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_StationLocationId",
                table: "GasStation",
                column: "StationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_StationStatusId",
                table: "GasStation",
                column: "StationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_TerritoryId",
                table: "GasStation",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_TradingHallOperatingModeId",
                table: "GasStation",
                column: "TradingHallOperatingModeId");

            migrationBuilder.CreateIndex(
                name: "IX_GasStation_TradingHallSizeId",
                table: "GasStation",
                column: "TradingHallSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_MeasureUnitId",
                table: "Inventory",
                column: "MeasureUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_NomenclatureId",
                table: "Inventory",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_GasStationId_Code",
                table: "Inventory",
                columns: new[] { "GasStationId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclature_MeasureUnitId",
                table: "Nomenclature",
                column: "MeasureUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclature_NomenclatureGroupId",
                table: "Nomenclature",
                column: "NomenclatureGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PersonId",
                table: "Order",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_GasStationId",
                table: "OrderDetail",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_NomenclatureId",
                table: "OrderDetail",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_AspNetUserId",
                table: "Person",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonTerritory_PersonId",
                table: "PersonTerritory",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonTerritory_RegionalStructureId",
                table: "PersonTerritory",
                column: "RegionalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalStructure_ParentId",
                table: "RegionalStructure",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StageInventory_GasStationId",
                table: "StageInventory",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_StageInventory_MeasureUnitId",
                table: "StageInventory",
                column: "MeasureUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StageInventory_PersonId",
                table: "StageInventory",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Action_PersonId",
                schema: "log",
                table: "Action",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Change_PersonId",
                schema: "log",
                table: "Change",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CalcSheet");

            migrationBuilder.DropTable(
                name: "CalcSheetHistory");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "PersonTerritory");

            migrationBuilder.DropTable(
                name: "StageInventory");

            migrationBuilder.DropTable(
                name: "Action",
                schema: "log");

            migrationBuilder.DropTable(
                name: "Change",
                schema: "log");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Nomenclature");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "GasStation");

            migrationBuilder.DropTable(
                name: "MeasureUnit",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "NomenclatureGroup",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "CashboxLocation",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "ClientRestroom",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "ManagementSystem",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "OperatorRoomFormat",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "ServiceLevel",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "Settlement",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "StationLocation",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "StationStatus",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "RegionalStructure");

            migrationBuilder.DropTable(
                name: "TradingHallOperatingMode",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "TradingHallSize",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
