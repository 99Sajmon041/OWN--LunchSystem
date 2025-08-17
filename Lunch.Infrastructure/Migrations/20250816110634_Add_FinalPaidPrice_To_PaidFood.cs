using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_FinalPaidPrice_To_PaidFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FinalPaidPrice",
                table: "PaidFoods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalPaidPrice",
                table: "PaidFoods");
        }
    }
}
