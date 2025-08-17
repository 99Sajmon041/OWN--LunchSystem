using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FillRollesTableUtility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoleDiscounts_RoleId",
                table: "RoleDiscounts");

            migrationBuilder.CreateIndex(
                name: "IX_RoleDiscounts_RoleId",
                table: "RoleDiscounts",
                column: "RoleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoleDiscounts_RoleId",
                table: "RoleDiscounts");

            migrationBuilder.CreateIndex(
                name: "IX_RoleDiscounts_RoleId",
                table: "RoleDiscounts",
                column: "RoleId");
        }
    }
}
