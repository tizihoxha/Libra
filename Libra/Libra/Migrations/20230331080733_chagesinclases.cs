using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libra.Migrations
{
    /// <inheritdoc />
    public partial class chagesinclases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookCount",
                table: "Author",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookCount",
                table: "Author");
        }
    }
}
