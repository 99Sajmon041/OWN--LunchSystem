using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToDailyOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "DailyOffer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "DailyOffer");
        }
    }
}
