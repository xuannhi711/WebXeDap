using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class AddSaleCampaigns : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "SaleCampaigns",
				columns: table => new
				{
					ID = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Description = table.Column<string>(type: "TEXT", nullable: true),
					DiscountType = table.Column<int>(type: "INTEGER", nullable: false),
					DiscountValue = table.Column<decimal>(type: "TEXT", nullable: false),
					StartsAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					EndsAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
					IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
					DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_SaleCampaigns", x => x.ID);
				}
			);

			migrationBuilder.CreateTable(
				name: "ProductSaleCampaign",
				columns: table => new
				{
					ProductsID = table.Column<int>(type: "INTEGER", nullable: false),
					SaleCampaignsID = table.Column<int>(type: "INTEGER", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey(
						"PK_ProductSaleCampaign",
						x => new { x.ProductsID, x.SaleCampaignsID }
					);
					table.ForeignKey(
						name: "FK_ProductSaleCampaign_Products_ProductsID",
						column: x => x.ProductsID,
						principalTable: "Products",
						principalColumn: "ID",
						onDelete: ReferentialAction.Cascade
					);
					table.ForeignKey(
						name: "FK_ProductSaleCampaign_SaleCampaigns_SaleCampaignsID",
						column: x => x.SaleCampaignsID,
						principalTable: "SaleCampaigns",
						principalColumn: "ID",
						onDelete: ReferentialAction.Cascade
					);
				}
			);

			migrationBuilder.CreateIndex(
				name: "IX_ProductSaleCampaign_SaleCampaignsID",
				table: "ProductSaleCampaign",
				column: "SaleCampaignsID"
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "ProductSaleCampaign");

			migrationBuilder.DropTable(name: "SaleCampaigns");
		}
	}
}
