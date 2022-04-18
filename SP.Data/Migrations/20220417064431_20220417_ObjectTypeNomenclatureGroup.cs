using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SP.Data.Migrations
{
    public partial class _20220417_ObjectTypeNomenclatureGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectTypeNomenclatureGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObjectType = table.Column<int>(nullable: false),
                    NomenclatureGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectTypeNomenclatureGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectTypeNomenclatureGroup_NomenclatureGroup_NomenclatureG~",
                        column: x => x.NomenclatureGroupId,
                        principalSchema: "dic",
                        principalTable: "NomenclatureGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectTypeNomenclatureGroup_NomenclatureGroupId",
                table: "ObjectTypeNomenclatureGroup",
                column: "NomenclatureGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectTypeNomenclatureGroup");
        }
    }
}
