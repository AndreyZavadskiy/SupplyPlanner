using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200525_GasStation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dic");

            migrationBuilder.CreateTable(
                name: "CashboxLocation",
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
                    table.PrimaryKey("PK_CashboxLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientRestroom",
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
                    table.PrimaryKey("PK_ClientRestroom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManagementSystem",
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
                    table.PrimaryKey("PK_ManagementSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatorRoomFormat",
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
                    table.PrimaryKey("PK_OperatorRoomFormat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLevel",
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
                    table.PrimaryKey("PK_ServiceLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationLocation",
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
                    table.PrimaryKey("PK_StationLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationStatus",
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
                    table.PrimaryKey("PK_StationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradingHallOperatingMode",
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
                    table.PrimaryKey("PK_TradingHallOperatingMode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradingHallSize",
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
                    table.PrimaryKey("PK_TradingHallSize", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "StationLocation",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "StationStatus",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "TradingHallOperatingMode",
                schema: "dic");

            migrationBuilder.DropTable(
                name: "TradingHallSize",
                schema: "dic");
        }
    }
}
