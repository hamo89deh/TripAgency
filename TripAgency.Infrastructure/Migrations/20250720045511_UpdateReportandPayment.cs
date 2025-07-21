using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportandPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_BookingTrips_BookingTripId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_BookingTripId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "BookingTripId",
                table: "Refunds");

            migrationBuilder.RenameColumn(
                name: "PackageTripDateId",
                table: "Notifications",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Refunds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedByUserId",
                table: "Refunds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundProcessedDate",
                table: "Refunds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "Refunds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransactionRefunded",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Refunds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NotificationType",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelatedEntityId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "PaymentDiscrepancyReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReportedTransactionRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReportedPaymentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportedPaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByUserId = table.Column<int>(type: "int", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDiscrepancyReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDiscrepancyReports_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentDiscrepancyReports_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PaymentDiscrepancyReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_PaymentId",
                table: "Refunds",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_ProcessedByUserId",
                table: "Refunds",
                column: "ProcessedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_ReportId",
                table: "Refunds",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDiscrepancyReports_PaymentId",
                table: "PaymentDiscrepancyReports",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDiscrepancyReports_ReviewedByUserId",
                table: "PaymentDiscrepancyReports",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDiscrepancyReports_UserId",
                table: "PaymentDiscrepancyReports",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_PaymentDiscrepancyReports_ReportId",
                table: "Refunds",
                column: "ReportId",
                principalTable: "PaymentDiscrepancyReports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Users_ProcessedByUserId",
                table: "Refunds",
                column: "ProcessedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_PaymentDiscrepancyReports_ReportId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Users_ProcessedByUserId",
                table: "Refunds");

            migrationBuilder.DropTable(
                name: "PaymentDiscrepancyReports");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_PaymentId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_ProcessedByUserId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_ReportId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "ProcessedByUserId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "RefundProcessedDate",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "TransactionRefunded",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RelatedEntityId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Notifications",
                newName: "PackageTripDateId");

            migrationBuilder.AddColumn<int>(
                name: "BookingTripId",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_BookingTripId",
                table: "Refunds",
                column: "BookingTripId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_BookingTrips_BookingTripId",
                table: "Refunds",
                column: "BookingTripId",
                principalTable: "BookingTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
