using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingPassnegerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingPassengers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    BookingTripId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPassengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingPassengers_BookingTrips_BookingTripId",
                        column: x => x.BookingTripId,
                        principalTable: "BookingTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_BookingTripId",
                table: "BookingPassengers",
                column: "BookingTripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingPassengers");
        }
    }
}
