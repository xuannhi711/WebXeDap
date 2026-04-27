using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Migrations
{
	/// <inheritdoc />
	public partial class themMau : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Maus",
				columns: table => new
				{
					Id = table
						.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					TenMau = table.Column<string>(type: "nvarchar(max)", nullable: false),
					MaSP = table.Column<string>(type: "nvarchar(450)", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Maus", x => x.Id);
					table.ForeignKey(
						name: "FK_Maus_Sanphams_MaSP",
						column: x => x.MaSP,
						principalTable: "Sanphams",
						principalColumn: "MaSP",
						onDelete: ReferentialAction.Cascade
					);
				}
			);

			migrationBuilder.CreateIndex(name: "IX_Maus_MaSP", table: "Maus", column: "MaSP");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "Maus");
		}
	}
}
