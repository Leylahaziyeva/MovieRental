using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieRental.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EventArtists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArtists_Events_EventId",
                table: "EventArtists");

            migrationBuilder.DropTable(
                name: "SportPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventArtists",
                table: "EventArtists");

            migrationBuilder.DropIndex(
                name: "IX_EventArtists_EventId",
                table: "EventArtists");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "EventArtists",
                newName: "EventsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventArtists",
                table: "EventArtists",
                columns: new[] { "EventsId", "ArtistsId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventArtists_ArtistsId",
                table: "EventArtists",
                column: "ArtistsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArtists_Events_EventsId",
                table: "EventArtists",
                column: "EventsId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArtists_Events_EventsId",
                table: "EventArtists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventArtists",
                table: "EventArtists");

            migrationBuilder.DropIndex(
                name: "IX_EventArtists_ArtistsId",
                table: "EventArtists");

            migrationBuilder.RenameColumn(
                name: "EventsId",
                table: "EventArtists",
                newName: "EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventArtists",
                table: "EventArtists",
                columns: new[] { "ArtistsId", "EventId" });

            migrationBuilder.CreateTable(
                name: "SportPlayers",
                columns: table => new
                {
                    PlayersId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportPlayers", x => new { x.PlayersId, x.SportId });
                    table.ForeignKey(
                        name: "FK_SportPlayers_Persons_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SportPlayers_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventArtists_EventId",
                table: "EventArtists",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SportPlayers_SportId",
                table: "SportPlayers",
                column: "SportId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArtists_Events_EventId",
                table: "EventArtists",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
