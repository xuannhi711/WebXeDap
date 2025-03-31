using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chitietgiohangs");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "Giohangs",
                newName: "SoLuong");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                table: "Giohangs",
                newName: "NgayThem");

            migrationBuilder.AddColumn<decimal>(
                name: "DonGia",
                table: "Giohangs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "Giohangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MaSP",
                table: "Giohangs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mau",
                table: "Giohangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Giohangs_MaSP",
                table: "Giohangs",
                column: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_Giohangs_Sanphams_MaSP",
                table: "Giohangs",
                column: "MaSP",
                principalTable: "Sanphams",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Giohangs_Sanphams_MaSP",
                table: "Giohangs");

            migrationBuilder.DropIndex(
                name: "IX_Giohangs_MaSP",
                table: "Giohangs");

            migrationBuilder.DropColumn(
                name: "DonGia",
                table: "Giohangs");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "Giohangs");

            migrationBuilder.DropColumn(
                name: "MaSP",
                table: "Giohangs");

            migrationBuilder.DropColumn(
                name: "Mau",
                table: "Giohangs");

            migrationBuilder.RenameColumn(
                name: "SoLuong",
                table: "Giohangs",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "NgayThem",
                table: "Giohangs",
                newName: "NgayTao");

            migrationBuilder.CreateTable(
                name: "Chitietgiohangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GioHangId = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chitietgiohangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chitietgiohangs_Giohangs_GioHangId",
                        column: x => x.GioHangId,
                        principalTable: "Giohangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Chitietgiohangs_Sanphams_MaSP",
                        column: x => x.MaSP,
                        principalTable: "Sanphams",
                        principalColumn: "MaSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chitietgiohangs_GioHangId",
                table: "Chitietgiohangs",
                column: "GioHangId");

            migrationBuilder.CreateIndex(
                name: "IX_Chitietgiohangs_MaSP",
                table: "Chitietgiohangs",
                column: "MaSP");
        }
    }
}
