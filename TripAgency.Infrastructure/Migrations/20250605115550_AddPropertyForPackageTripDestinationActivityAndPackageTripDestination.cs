using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyForPackageTripDestinationActivityAndPackageTripDestination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayNumber",
                table: "PackageTripDestinations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PackageTripDestinations",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "PackageTripDestinations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "PackageTripDestinations",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "OrderDestination",
                table: "PackageTripDestinations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "PackageTripDestinations",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PackageTripDestinationActivities",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "PackageTripDestinationActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "PackageTripDestinationActivities",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "OrderActivity",
                table: "PackageTripDestinationActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "PackageTripDestinationActivities",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayNumber",
                table: "PackageTripDestinations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PackageTripDestinations");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "PackageTripDestinations");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "PackageTripDestinations");

            migrationBuilder.DropColumn(
                name: "OrderDestination",
                table: "PackageTripDestinations");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "PackageTripDestinations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PackageTripDestinationActivities");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "PackageTripDestinationActivities");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "PackageTripDestinationActivities");

            migrationBuilder.DropColumn(
                name: "OrderActivity",
                table: "PackageTripDestinationActivities");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "PackageTripDestinationActivities");
        }
    }
}
