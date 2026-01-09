using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReaderId",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReaderId",
                table: "Reviews",
                column: "ReaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_ReaderId",
                table: "Reviews",
                column: "ReaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_ReaderId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReaderId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReaderId",
                table: "Reviews");
        }
    }
}
