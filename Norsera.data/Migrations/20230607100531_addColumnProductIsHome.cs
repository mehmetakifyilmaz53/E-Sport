using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Norsera.data.Migrations
{
    /// <inheritdoc />
    public partial class addColumnProductIsHome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHome",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHome",
                table: "Products");
        }
    }
}
