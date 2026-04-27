using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Migrations
{
	/// <inheritdoc />
	public partial class tintuc : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "tintuc",
				columns: table => new
				{
					id = table
						.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					tieude = table.Column<string>(type: "nvarchar(max)", nullable: false),
					noidung = table.Column<string>(type: "nvarchar(max)", nullable: false),
					hinhanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ngaytao = table.Column<DateTime>(type: "datetime2", nullable: true),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_tintuc", x => x.id);
				}
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "tintuc");
		}
	}
}
