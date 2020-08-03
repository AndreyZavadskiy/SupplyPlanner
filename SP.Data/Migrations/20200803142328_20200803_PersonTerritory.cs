using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200803_PersonTerritory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_PersonTerritory_PersonId",
                table: "PersonTerritory",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonTerritory_RegionalStructureId",
                table: "PersonTerritory",
                column: "RegionalStructureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonTerritory");
        }
    }
}
