using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderBackend.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NavigationSessions",
                columns: table => new
                {
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationSessions", x => x.SessionId);
                });

            migrationBuilder.CreateTable(
                name: "NavigationSteps",
                columns: table => new
                {
                    NavigationSessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationSteps", x => new { x.NavigationSessionId, x.Order });
                    table.ForeignKey(
                        name: "FK_NavigationSteps_NavigationSessions_NavigationSessionId",
                        column: x => x.NavigationSessionId,
                        principalTable: "NavigationSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NavigationSteps");

            migrationBuilder.DropTable(
                name: "NavigationSessions");
        }
    }
}
