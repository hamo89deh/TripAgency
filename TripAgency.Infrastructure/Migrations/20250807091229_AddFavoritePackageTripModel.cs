using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritePackageTripModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoritePackageTrips",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PackageTripId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePackageTrips", x => new { x.UserId, x.PackageTripId });
                    table.ForeignKey(
                        name: "FK_FavoritePackageTrips_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FavoritePackageTrips_PackageTrips_PackageTripId",
                        column: x => x.PackageTripId,
                        principalTable: "PackageTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePackageTrips_PackageTripId",
                table: "FavoritePackageTrips",
                column: "PackageTripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritePackageTrips");
        }
    }
}
