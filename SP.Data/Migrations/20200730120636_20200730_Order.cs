using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200730_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FixedAmount",
                table: "Requirement",
                type: "decimal(19,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
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
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.AlterColumn<decimal>(
                name: "FixedAmount",
                table: "Requirement",
                type: "decimal(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)",
                oldNullable: true);
        }
    }
}
