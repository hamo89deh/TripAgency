using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionPackageTripTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_PackageTrips_PackageTripId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_PackageTripId_Unique",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "PackageTripId",
                table: "Promotions");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionPackageTrips");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Promotions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PackageTripId",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_PackageTripId_Unique",
                table: "Promotions",
                column: "PackageTripId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_PackageTrips_PackageTripId",
                table: "Promotions",
                column: "PackageTripId",
                principalTable: "PackageTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
