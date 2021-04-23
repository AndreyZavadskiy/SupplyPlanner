using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Data.Migrations
{
    public partial class _20210422_Segment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "SegmentId",
                table: "GasStation",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation",
                column: "SegmentId",
                principalSchema: "dic",
                principalTable: "Segment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation");

            migrationBuilder.AlterColumn<int>(
                name: "SegmentId",
                table: "GasStation",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_GasStation_Segment_SegmentId",
                table: "GasStation",
                column: "SegmentId",
                principalSchema: "dic",
                principalTable: "Segment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
