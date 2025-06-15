using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingTripAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TripDates",
                type: "int",
                nullable: false,
                comment: "Represents trip status: 0 = Available, 1 = Completed, 2 = Cancelled, 3 = Planned",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Represents trip status: 0 = Pending, 1 = Available, 2 = Completed, 3 = Cancelled");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoyaltyPoints = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassengerCount = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingStatus = table.Column<int>(type: "int", nullable: false, comment: "Represents Booking status: 0 = Pending, 1 = Completed, 2 = Cancelled"),
                    ActualPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TripDateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingTrips_TripDates_TripDateId",
                        column: x => x.TripDateId,
                        principalTable: "TripDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingTrips_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingTrips_TripDateId",
                table: "BookingTrips",
                column: "TripDateId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTrips_UserId",
                table: "BookingTrips",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingTrips");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TripDates",
                type: "int",
                nullable: false,
                comment: "Represents trip status: 0 = Pending, 1 = Available, 2 = Completed, 3 = Cancelled",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Represents trip status: 0 = Available, 1 = Completed, 2 = Cancelled, 3 = Planned");
        }
    }
}
