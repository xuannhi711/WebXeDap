using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Migrations
{
    /// <inheritdoc />
    public partial class giakm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaKhuyenMai",
                table: "Sanphams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GiaKhuyenMai",
                table: "Sanphams",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
