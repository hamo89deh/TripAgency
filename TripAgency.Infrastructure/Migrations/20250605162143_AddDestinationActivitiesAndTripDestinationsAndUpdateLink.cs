using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDestinationActivitiesAndTripDestinationsAndUpdateLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageTripDestinationActivities_Activities_ActivityId",
                table: "PackageTripDestinationActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageTripDestinations_Destinations_DestinationId",
                table: "PackageTripDestinations");

            migrationBuilder.RenameColumn(
                name: "DestinationId",
                table: "PackageTripDestinations",
                newName: "TripDestinationId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTripDestinations_DestinationId",
                table: "PackageTripDestinations",
                newName: "IX_PackageTripDestinations_TripDestinationId");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                table: "PackageTripDestinationActivities",
                newName: "DestinationActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTripDestinationActivities_ActivityId",
                table: "PackageTripDestinationActivities",
                newName: "IX_PackageTripDestinationActivities_DestinationActivityId");

            migrationBuilder.CreateTable(
                name: "DestinationActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinationActivities_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DestinationActivities_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TripDestinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripDestinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripDestinations_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TripDestinations_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DestinationActivities_ActivityId",
                table: "DestinationActivities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationActivities_DestinationId",
                table: "DestinationActivities",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDestinations_DestinationId",
                table: "TripDestinations",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDestinations_TripId",
                table: "TripDestinations",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTripDestinationActivities_DestinationActivities_DestinationActivityId",
                table: "PackageTripDestinationActivities",
                column: "DestinationActivityId",
                principalTable: "DestinationActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTripDestinations_TripDestinations_TripDestinationId",
                table: "PackageTripDestinations",
                column: "TripDestinationId",
                principalTable: "TripDestinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageTripDestinationActivities_DestinationActivities_DestinationActivityId",
                table: "PackageTripDestinationActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageTripDestinations_TripDestinations_TripDestinationId",
                table: "PackageTripDestinations");

            migrationBuilder.DropTable(
                name: "DestinationActivities");

            migrationBuilder.DropTable(
                name: "TripDestinations");

            migrationBuilder.RenameColumn(
                name: "TripDestinationId",
                table: "PackageTripDestinations",
                newName: "DestinationId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTripDestinations_TripDestinationId",
                table: "PackageTripDestinations",
                newName: "IX_PackageTripDestinations_DestinationId");

            migrationBuilder.RenameColumn(
                name: "DestinationActivityId",
                table: "PackageTripDestinationActivities",
                newName: "ActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageTripDestinationActivities_DestinationActivityId",
                table: "PackageTripDestinationActivities",
                newName: "IX_PackageTripDestinationActivities_ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTripDestinationActivities_Activities_ActivityId",
                table: "PackageTripDestinationActivities",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageTripDestinations_Destinations_DestinationId",
                table: "PackageTripDestinations",
                column: "DestinationId",
                principalTable: "Destinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
