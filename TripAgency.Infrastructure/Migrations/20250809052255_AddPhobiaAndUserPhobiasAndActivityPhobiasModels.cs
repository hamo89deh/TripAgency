using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripAgency.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhobiaAndUserPhobiasAndActivityPhobiasModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Phobias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phobias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityPhobias",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    PhobiaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPhobias", x => new { x.ActivityId, x.PhobiaId });
                    table.ForeignKey(
                        name: "FK_ActivityPhobias_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityPhobias_Phobias_PhobiaId",
                        column: x => x.PhobiaId,
                        principalTable: "Phobias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPhobias",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PhobiaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhobias", x => new { x.UserId, x.PhobiaId });
                    table.ForeignKey(
                        name: "FK_UserPhobias_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPhobias_Phobias_PhobiaId",
                        column: x => x.PhobiaId,
                        principalTable: "Phobias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPhobias_PhobiaId",
                table: "ActivityPhobias",
                column: "PhobiaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhobias_PhobiaId",
                table: "UserPhobias",
                column: "PhobiaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityPhobias");

            migrationBuilder.DropTable(
                name: "UserPhobias");

            migrationBuilder.DropTable(
                name: "Phobias");
        }
    }
}
