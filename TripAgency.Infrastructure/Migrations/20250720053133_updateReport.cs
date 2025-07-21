using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentDiscrepancyReports_Payments_PaymentId",
                table: "PaymentDiscrepancyReports");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "PaymentDiscrepancyReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentDiscrepancyReports_Payments_PaymentId",
                table: "PaymentDiscrepancyReports",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentDiscrepancyReports_Payments_PaymentId",
                table: "PaymentDiscrepancyReports");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "PaymentDiscrepancyReports",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentDiscrepancyReports_Payments_PaymentId",
                table: "PaymentDiscrepancyReports",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
