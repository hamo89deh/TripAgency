using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationAndRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTrips_TripDates_TripDateId",
                table: "BookingTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_TripDates_PackageTrips_PackageTripId",
                table: "TripDates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TripDates",
                table: "TripDates");

            migrationBuilder.RenameTable(
                name: "TripDates",
                newName: "PackageTripDates");

            migrationBuilder.RenameColumn(
                name: "TripDateId",
                table: "BookingTrips",
                newName: "PackageTripDateId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingTrips_TripDateId",
                table: "BookingTrips",
                newName: "IX_BookingTrips_PackageTripDateId");

            migrationBuilder.RenameIndex(
                name: "IX_TripDates_PackageTripId",
                table: "PackageTripDates",
                newName: "IX_PackageTripDates_PackageTripId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PackageTripDates",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Represents trip status: 0 = Available, 1 = Completed, 2 = Cancelled, 3 = Planned");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageTripDates",
                table: "PackageTripDates",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageTripDateId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingTripId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, comment: " status: 0 = Pending, 1 = Completed, 2 = Failed")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refunds_BookingTrips_BookingTripId",
                        column: x => x.BookingTripId,
                        principalTable: "BookingTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_BookingTripId",
                table: "Refunds",
                column: "BookingTripId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTrips_PackageTripDates_PackageTripDateId",
                table: "BookingTrips",
                column: "PackageTripDateId",
                principalTable: "PackageTripDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTripDates_PackageTrips_PackageTripId",
                table: "PackageTripDates",
                column: "PackageTripId",
                principalTable: "PackageTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTrips_PackageTripDates_PackageTripDateId",
                table: "BookingTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageTripDates_PackageTrips_PackageTripId",
                table: "PackageTripDates");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageTripDates",
                table: "PackageTripDates");

            migrationBuilder.RenameTable(
                name: "PackageTripDates",
                newName: "TripDates");

            migrationBuilder.RenameColumn(
                name: "PackageTripDateId",
                table: "BookingTrips",
                newName: "TripDateId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingTrips_PackageTripDateId",
                table: "BookingTrips",
                newName: "IX_BookingTrips_TripDateId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTripDates_PackageTripId",
                table: "TripDates",
                newName: "IX_TripDates_PackageTripId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TripDates",
                type: "int",
                nullable: false,
                comment: "Represents trip status: 0 = Available, 1 = Completed, 2 = Cancelled, 3 = Planned",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TripDates",
                table: "TripDates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTrips_TripDates_TripDateId",
                table: "BookingTrips",
                column: "TripDateId",
                principalTable: "TripDates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TripDates_PackageTrips_PackageTripId",
                table: "TripDates",
                column: "PackageTripId",
                principalTable: "PackageTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
