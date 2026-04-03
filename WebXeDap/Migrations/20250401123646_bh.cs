using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Migrations
{
    /// <inheritdoc />
    public partial class bh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baohanhs_Chitiethoadons_chitiethoadonId",
                table: "Baohanhs");

            migrationBuilder.DropIndex(
                name: "IX_Baohanhs_chitiethoadonId",
                table: "Baohanhs");

            migrationBuilder.DropColumn(
                name: "chitiethoadonId",
                table: "Baohanhs");

            migrationBuilder.AlterColumn<int>(
                name: "MaHD",
                table: "Baohanhs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayHetHanBaoHanh",
                table: "Baohanhs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Baohanhs_MaHD",
                table: "Baohanhs",
                column: "MaHD");

            migrationBuilder.AddForeignKey(
                name: "FK_Baohanhs_Hoadon_MaHD",
                table: "Baohanhs",
                column: "MaHD",
                principalTable: "Hoadon",
                principalColumn: "MaHD",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baohanhs_Hoadon_MaHD",
                table: "Baohanhs");

            migrationBuilder.DropIndex(
                name: "IX_Baohanhs_MaHD",
                table: "Baohanhs");

            migrationBuilder.DropColumn(
                name: "NgayHetHanBaoHanh",
                table: "Baohanhs");

            migrationBuilder.AlterColumn<string>(
                name: "MaHD",
                table: "Baohanhs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "chitiethoadonId",
                table: "Baohanhs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Baohanhs_chitiethoadonId",
                table: "Baohanhs",
                column: "chitiethoadonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baohanhs_Chitiethoadons_chitiethoadonId",
                table: "Baohanhs",
                column: "chitiethoadonId",
                principalTable: "Chitiethoadons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
