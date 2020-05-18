using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20200518_Person_User_relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Person_AspNetUserId",
                table: "Person",
                column: "AspNetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_AspNetUsers_AspNetUserId",
                table: "Person",
                column: "AspNetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_AspNetUsers_AspNetUserId",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_AspNetUserId",
                table: "Person");
        }
    }
}
