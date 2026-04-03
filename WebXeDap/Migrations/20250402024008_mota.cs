using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Migrations
{
    /// <inheritdoc />
    public partial class mota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mota",
                table: "Sanphams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mota",
                table: "Sanphams");
        }
    }
}
