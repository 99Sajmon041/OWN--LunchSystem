using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RepareLowerCaseOfentityMustChangePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "mustChangePassword",
                table: "AspNetUsers",
                newName: "MustChangePassword");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MustChangePassword",
                table: "AspNetUsers",
                newName: "mustChangePassword");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);
        }
    }
}
