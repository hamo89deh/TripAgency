using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripPackageTypesAndDeletLinkTripAndTripType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_TypeTrips_TypeTripId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_TypeTripId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "TypeTripId",
                table: "Trips");

            migrationBuilder.CreateTable(
                name: "PackageTripTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageTripId = table.Column<int>(type: "int", nullable: false),
                    TypeTripId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageTripTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageTripTypes_PackageTrips_PackageTripId",
                        column: x => x.PackageTripId,
                        principalTable: "PackageTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageTripTypes_TypeTrips_TypeTripId",
                        column: x => x.TypeTripId,
                        principalTable: "TypeTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageTripTypes_PackageTripId",
                table: "PackageTripTypes",
                column: "PackageTripId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageTripTypes_TypeTripId",
                table: "PackageTripTypes",
                column: "TypeTripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageTripTypes");

            migrationBuilder.AddColumn<int>(
                name: "TypeTripId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TypeTripId",
                table: "Trips",
                column: "TypeTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_TypeTrips_TypeTripId",
                table: "Trips",
                column: "TypeTripId",
                principalTable: "TypeTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
