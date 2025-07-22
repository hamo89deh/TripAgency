using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageTripDestinationActivities_DestinationActivities_DestinationActivityId",
                table: "PackageTripDestinationActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageTripDestinations_TripDestinations_TripDestinationId",
                table: "PackageTripDestinations");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
