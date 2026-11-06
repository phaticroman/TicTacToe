using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Board = table.Column<string>(type: "char(9)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CurrentTurn = table.Column<string>(type: "char(1)", nullable: true),
                    WinnerSymbol = table.Column<string>(type: "char(1)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRooms", x => x.Id);
                    table.CheckConstraint("CK_GameRooms_Board", "LEN([Board]) = 9 AND [Board] NOT LIKE '%[^XO-]%'");
                    table.CheckConstraint("CK_GameRooms_Status", "[Status] IN ('WaitingForPlayer', 'InProgress', 'Won', 'Draw')");
                });

            migrationBuilder.CreateTable(
                name: "GamePlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Symbol = table.Column<string>(type: "char(1)", nullable: false),
                    IsConnected = table.Column<bool>(type: "bit", nullable: false),
                    WantsRematch = table.Column<bool>(type: "bit", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayers", x => x.Id);
                    table.CheckConstraint("CK_GamePlayers_Symbol", "[Symbol] IN ('X', 'O')");
                    table.ForeignKey(
                        name: "FK_GamePlayers_GameRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_RoomId_Symbol",
                table: "GamePlayers",
                columns: new[] { "RoomId", "Symbol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameRooms_RoomCode",
                table: "GameRooms",
                column: "RoomCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamePlayers");

            migrationBuilder.DropTable(
                name: "GameRooms");
        }
    }
}
