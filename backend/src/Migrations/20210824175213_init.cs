using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace uptimey.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseTime = table.Column<double>(type: "double precision", nullable: false),
                    HasError = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    UserSiteId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateChecked = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteReports_UserSites_UserSiteId",
                        column: x => x.UserSiteId,
                        principalTable: "UserSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteReports_UserSiteId",
                table: "SiteReports",
                column: "UserSiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteReports");

            migrationBuilder.DropTable(
                name: "UserSites");
        }
    }
}
