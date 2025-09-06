using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditTableNameForPromotionToOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTrips_Promotions_AppliedPromotionId",
                table: "BookingTrips");

            migrationBuilder.DropTable(
                name: "PromotionPackageTrips");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.RenameColumn(
                name: "AppliedPromotionId",
                table: "BookingTrips",
                newName: "AppliedOfferId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingTrips_AppliedPromotionId",
                table: "BookingTrips",
                newName: "IX_BookingTrips_AppliedOfferId");

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageTripOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageTripId = table.Column<int>(type: "int", nullable: false),
                    OfferId = table.Column<int>(type: "int", nullable: false),
                    IsApply = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageTripOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageTripOffers_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageTripOffers_PackageTrips_PackageTripId",
                        column: x => x.PackageTripId,
                        principalTable: "PackageTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageTripOffers_OfferId",
                table: "PackageTripOffers",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageTripOffers_PackageTripId",
                table: "PackageTripOffers",
                column: "PackageTripId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTrips_Offers_AppliedOfferId",
                table: "BookingTrips",
                column: "AppliedOfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTrips_Offers_AppliedOfferId",
                table: "BookingTrips");

            migrationBuilder.DropTable(
                name: "PackageTripOffers");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.RenameColumn(
                name: "AppliedOfferId",
                table: "BookingTrips",
                newName: "AppliedPromotionId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingTrips_AppliedOfferId",
                table: "BookingTrips",
                newName: "IX_BookingTrips_AppliedPromotionId");

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromotionPackageTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageTripId = table.Column<int>(type: "int", nullable: false),
                    PromotionId = table.Column<int>(type: "int", nullable: false),
                    IsApply = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionPackageTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionPackageTrips_PackageTrips_PackageTripId",
                        column: x => x.PackageTripId,
                        principalTable: "PackageTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionPackageTrips_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionPackageTrips_PackageTripId",
                table: "PromotionPackageTrips",
                column: "PackageTripId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionPackageTrips_PromotionId",
                table: "PromotionPackageTrips",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTrips_Promotions_AppliedPromotionId",
                table: "BookingTrips",
                column: "AppliedPromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
