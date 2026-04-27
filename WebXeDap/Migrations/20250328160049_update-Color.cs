using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Migrations
{
	/// <inheritdoc />
	public partial class updateColor : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Sanphams_Khuyenmais_MaKMC",
				table: "Sanphams"
			);

			migrationBuilder.DropIndex(name: "IX_Sanphams_MaKMC", table: "Sanphams");

			migrationBuilder.DropColumn(name: "MaKMC", table: "Sanphams");

			migrationBuilder.AlterColumn<string>(
				name: "MaKM",
				table: "Sanphams",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)"
			);

			migrationBuilder.AddColumn<string>(
				name: "Mausac",
				table: "Chitiethoadons",
				type: "nvarchar(max)",
				nullable: true
			);

			migrationBuilder.CreateIndex(
				name: "IX_Sanphams_MaKM",
				table: "Sanphams",
				column: "MaKM"
			);

			migrationBuilder.AddForeignKey(
				name: "FK_Sanphams_Khuyenmais_MaKM",
				table: "Sanphams",
				column: "MaKM",
				principalTable: "Khuyenmais",
				principalColumn: "MaKM",
				onDelete: ReferentialAction.Cascade
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(name: "FK_Sanphams_Khuyenmais_MaKM", table: "Sanphams");

			migrationBuilder.DropIndex(name: "IX_Sanphams_MaKM", table: "Sanphams");

			migrationBuilder.DropColumn(name: "Mausac", table: "Chitiethoadons");

			migrationBuilder.AlterColumn<string>(
				name: "MaKM",
				table: "Sanphams",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)"
			);

			migrationBuilder.AddColumn<string>(
				name: "MaKMC",
				table: "Sanphams",
				type: "nvarchar(450)",
				nullable: true
			);

			migrationBuilder.CreateIndex(
				name: "IX_Sanphams_MaKMC",
				table: "Sanphams",
				column: "MaKMC"
			);

			migrationBuilder.AddForeignKey(
				name: "FK_Sanphams_Khuyenmais_MaKMC",
				table: "Sanphams",
				column: "MaKMC",
				principalTable: "Khuyenmais",
				principalColumn: "MaKM"
			);
		}
	}
}
