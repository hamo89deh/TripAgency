using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionAndTripReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppliedPromotionId",
                table: "BookingTrips",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageTripId = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotions_PackageTrips_PackageTripId",
                        column: x => x.PackageTripId,
                        principalTable: "PackageTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TripReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageTripDateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TripReviews_PackageTripDates_PackageTripDateId",
                        column: x => x.PackageTripDateId,
                        principalTable: "PackageTripDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingTrips_AppliedPromotionId",
                table: "BookingTrips",
                column: "AppliedPromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_PackageTripId_Unique",
                table: "Promotions",
                column: "PackageTripId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripReviews_PackageTripDateId",
                table: "TripReviews",
                column: "PackageTripDateId");

            migrationBuilder.CreateIndex(
                name: "IX_TripReviews_UserId_PackageTripDateId_Unique",
                table: "TripReviews",
                columns: new[] { "UserId", "PackageTripDateId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTrips_Promotions_AppliedPromotionId",
                table: "BookingTrips",
                column: "AppliedPromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTrips_Promotions_AppliedPromotionId",
                table: "BookingTrips");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "TripReviews");

            migrationBuilder.DropIndex(
                name: "IX_BookingTrips_AppliedPromotionId",
                table: "BookingTrips");

            migrationBuilder.DropColumn(
                name: "AppliedPromotionId",
                table: "BookingTrips");
        }
    }
}
