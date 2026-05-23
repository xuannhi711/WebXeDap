using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebXeDap.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class AddPayments : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(name: "IsEnabled", table: "SaleCampaigns");

			migrationBuilder.CreateTable(
				name: "Payments",
				columns: table => new
				{
					ID = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					OrderID = table.Column<int>(type: "INTEGER", nullable: false),
					Provider = table.Column<int>(type: "INTEGER", nullable: false),
					Status = table.Column<int>(type: "INTEGER", nullable: false),
					Amount = table.Column<decimal>(type: "TEXT", nullable: false),
					CurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
					ReferenceCode = table.Column<string>(type: "TEXT", nullable: false),
					ProviderTransactionID = table.Column<string>(type: "TEXT", nullable: true),
					ProviderPaymentUrl = table.Column<string>(type: "TEXT", nullable: true),
					FailureReason = table.Column<string>(type: "TEXT", nullable: true),
					RawResponse = table.Column<string>(type: "TEXT", nullable: true),
					CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
					IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
					DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Payments", x => x.ID);
					table.ForeignKey(
						name: "FK_Payments_Orders_OrderID",
						column: x => x.OrderID,
						principalTable: "Orders",
						principalColumn: "ID",
						onDelete: ReferentialAction.Cascade
					);
				}
			);

			migrationBuilder.CreateIndex(
				name: "IX_Payments_OrderID",
				table: "Payments",
				column: "OrderID"
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "Payments");

			migrationBuilder.AddColumn<bool>(
				name: "IsEnabled",
				table: "SaleCampaigns",
				type: "INTEGER",
				nullable: false,
				defaultValue: false
			);
		}
	}
}
