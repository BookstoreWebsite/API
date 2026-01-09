using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterReportsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_UserId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reports",
                newName: "ReaderId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_UserId",
                table: "Reports",
                newName: "IX_Reports_ReaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_ReaderId",
                table: "Reports",
                column: "ReaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_ReaderId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReaderId",
                table: "Reports",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReaderId",
                table: "Reports",
                newName: "IX_Reports_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_UserId",
                table: "Reports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
