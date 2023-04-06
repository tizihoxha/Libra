using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libra.Migrations
{
    /// <inheritdoc />
    public partial class removedfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookCount",
                table: "Author");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookCount",
                table: "Author",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
