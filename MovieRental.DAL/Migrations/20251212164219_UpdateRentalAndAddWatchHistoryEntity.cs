using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieRental.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentalAndAddWatchHistoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals");

            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Movies_MovieId",
                table: "Rentals");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstWatchedAt",
                table: "Rentals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Rentals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWatchedAt",
                table: "Rentals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Rentals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Rentals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Rentals",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalWatchTimeSeconds",
                table: "Rentals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Rentals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WatchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    RentalId = table.Column<int>(type: "int", nullable: true),
                    WatchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WatchDurationSeconds = table.Column<int>(type: "int", nullable: false),
                    TotalDurationSeconds = table.Column<int>(type: "int", nullable: false),
                    CompletionPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WatchHistories_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WatchHistories_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_ExpiryDate",
                table: "Rentals",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_Status",
                table: "Rentals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_UserId_Status",
                table: "Rentals",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_MovieId",
                table: "WatchHistories",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_RentalId",
                table: "WatchHistories",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_UserId",
                table: "WatchHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_UserId_WatchedAt",
                table: "WatchHistories",
                columns: new[] { "UserId", "WatchedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_WatchedAt",
                table: "WatchHistories",
                column: "WatchedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Movies_MovieId",
                table: "Rentals",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals");

            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Movies_MovieId",
                table: "Rentals");

            migrationBuilder.DropTable(
                name: "WatchHistories");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_ExpiryDate",
                table: "Rentals");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_Status",
                table: "Rentals");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_UserId_Status",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "FirstWatchedAt",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "LastWatchedAt",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "TotalWatchTimeSeconds",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Rentals");

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Movies_MovieId",
                table: "Rentals",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
