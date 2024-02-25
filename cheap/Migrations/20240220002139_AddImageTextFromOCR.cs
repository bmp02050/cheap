using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cheap.Migrations
{
    /// <inheritdoc />
    public partial class AddImageTextFromOCR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageText",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageText",
                table: "Items");
        }
    }
}
