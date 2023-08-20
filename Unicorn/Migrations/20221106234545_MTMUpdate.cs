using Microsoft.EntityFrameworkCore.Migrations;

namespace Unicorn.Migrations
{
    public partial class MTMUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarPart",
                columns: table => new
                {
                    CarsId = table.Column<int>(type: "integer", nullable: false),
                    PartsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPart", x => new { x.CarsId, x.PartsId });
                    table.ForeignKey(
                        name: "FK_CarPart_Cars_CarsId",
                        column: x => x.CarsId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarPart_Parts_PartsId",
                        column: x => x.PartsId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarPart_PartsId",
                table: "CarPart",
                column: "PartsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarPart");
        }
    }
}
