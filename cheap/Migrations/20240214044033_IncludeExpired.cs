using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cheap.Migrations
{
    /// <inheritdoc />
    public partial class IncludeExpired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Expired",
                table: "TokenRepository",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expired",
                table: "TokenRepository");
        }
    }
}
