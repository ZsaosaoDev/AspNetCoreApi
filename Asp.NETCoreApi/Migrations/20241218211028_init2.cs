using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.NETCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToBuyLaters_AspNetUsers_UserId",
                table: "ToBuyLaters");

            migrationBuilder.DropIndex(
                name: "IX_ToBuyLaters_UserId",
                table: "ToBuyLaters");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ToBuyLaters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ToBuyLaters",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ToBuyLaters_UserId",
                table: "ToBuyLaters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToBuyLaters_AspNetUsers_UserId",
                table: "ToBuyLaters",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
