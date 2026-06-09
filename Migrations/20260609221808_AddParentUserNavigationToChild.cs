using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerienspassWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddParentUserNavigationToChild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ParentUserId",
                table: "Children",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Children_ParentUserId",
                table: "Children",
                column: "ParentUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_ParentUserId",
                table: "Children",
                column: "ParentUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_ParentUserId",
                table: "Children");

            migrationBuilder.DropIndex(
                name: "IX_Children_ParentUserId",
                table: "Children");

            migrationBuilder.AlterColumn<string>(
                name: "ParentUserId",
                table: "Children",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
