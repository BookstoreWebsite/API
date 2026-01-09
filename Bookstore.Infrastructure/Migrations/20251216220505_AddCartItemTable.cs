using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCartItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ShoppingCart_ShoppingCartUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_Users_UserId",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_Products_ShoppingCartUserId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingCart",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "ShoppingCartUserId",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "ShoppingCart",
                newName: "ShoppingCarts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingCarts",
                table: "ShoppingCarts",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShoppingCartUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_ShoppingCarts_ShoppingCartUserId",
                        column: x => x.ShoppingCartUserId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ShoppingCartUserId_ProductId",
                table: "CartItems",
                columns: new[] { "ShoppingCartUserId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Users_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Users_UserId",
                table: "ShoppingCarts");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingCarts",
                table: "ShoppingCarts");

            migrationBuilder.RenameTable(
                name: "ShoppingCarts",
                newName: "ShoppingCart");

            migrationBuilder.AddColumn<Guid>(
                name: "ShoppingCartUserId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingCart",
                table: "ShoppingCart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShoppingCartUserId",
                table: "Products",
                column: "ShoppingCartUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ShoppingCart_ShoppingCartUserId",
                table: "Products",
                column: "ShoppingCartUserId",
                principalTable: "ShoppingCart",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_Users_UserId",
                table: "ShoppingCart",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
