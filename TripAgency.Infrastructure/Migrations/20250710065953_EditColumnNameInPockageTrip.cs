using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditColumnNameInPockageTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTripDate",
                table: "TripDates",
                newName: "StartPackageTripDate");

            migrationBuilder.RenameColumn(
                name: "EndTripDate",
                table: "TripDates",
                newName: "EndPackageTripDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartPackageTripDate",
                table: "TripDates",
                newName: "StartTripDate");

            migrationBuilder.RenameColumn(
                name: "EndPackageTripDate",
                table: "TripDates",
                newName: "EndTripDate");
        }
    }
}
