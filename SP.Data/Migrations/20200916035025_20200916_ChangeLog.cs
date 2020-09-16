using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200916_ChangeLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Change",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(nullable: false),
                    ChangeDate = table.Column<DateTime>(nullable: false),
                    EntityName = table.Column<string>(nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_CalcSheetHistory_GasStationId",
                table: "CalcSheetHistory",
                column: "GasStationId");

            migrationBuilder.CreateIndex(
                name: "IX_CalcSheetHistory_NomenclatureId",
                table: "CalcSheetHistory",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Change_PersonId",
                schema: "log",
                table: "Change",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalcSheetHistory_GasStation_GasStationId",
                table: "CalcSheetHistory",
                column: "GasStationId",
                principalTable: "GasStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalcSheetHistory_Nomenclature_NomenclatureId",
                table: "CalcSheetHistory",
                column: "NomenclatureId",
                principalTable: "Nomenclature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalcSheetHistory_GasStation_GasStationId",
                table: "CalcSheetHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_CalcSheetHistory_Nomenclature_NomenclatureId",
                table: "CalcSheetHistory");

            migrationBuilder.DropTable(
                name: "Change",
                schema: "log");

            migrationBuilder.DropIndex(
                name: "IX_CalcSheetHistory_GasStationId",
                table: "CalcSheetHistory");

            migrationBuilder.DropIndex(
                name: "IX_CalcSheetHistory_NomenclatureId",
                table: "CalcSheetHistory");
        }
    }
}
