using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalPriceToWritteOffFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WrittenOff",
                table: "WrittenOffFoods");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAfterDiscount",
                table: "WrittenOffFoods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceAfterDiscount",
                table: "WrittenOffFoods");

            migrationBuilder.AddColumn<bool>(
                name: "WrittenOff",
                table: "WrittenOffFoods",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
