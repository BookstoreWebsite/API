using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReaderIdColumnToPurchaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Users_UserId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_UserId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Purchases");

            migrationBuilder.AddColumn<Guid>(
                name: "ReaderId",
                table: "Purchases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ReaderId",
                table: "Purchases",
                column: "ReaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Users_ReaderId",
                table: "Purchases",
                column: "ReaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Users_ReaderId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_ReaderId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ReaderId",
                table: "Purchases");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_UserId",
                table: "Purchases",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Users_UserId",
                table: "Purchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
