using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MapDailyOfferName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaidFoods_CustomerId",
                table: "PaidFoods");

            migrationBuilder.AlterColumn<string>(
                name: "CardId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PaidFoods_CustomerId_DailyOfferId",
                table: "PaidFoods",
                columns: new[] { "CustomerId", "DailyOfferId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CardId",
                table: "AspNetUsers",
                column: "CardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaidFoods_CustomerId_DailyOfferId",
                table: "PaidFoods");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CardId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "CardId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_PaidFoods_CustomerId",
                table: "PaidFoods",
                column: "CustomerId");
        }
    }
}
