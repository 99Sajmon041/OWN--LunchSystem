using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableRoleDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleDiscounts_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleDiscounts_RoleId",
                table: "RoleDiscounts",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleDiscounts");
        }
    }
}
