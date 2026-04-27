using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Migrations
{
	/// <inheritdoc />
	public partial class TT_HD : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "TrangThai",
				table: "Hoadon",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: ""
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(name: "TrangThai", table: "Hoadon");
		}
	}
}
