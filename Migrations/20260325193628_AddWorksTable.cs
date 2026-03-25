using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshowcaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWorksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "works",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrimaryCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SpaceType = table.Column<string>(type: "text", nullable: false),
                    CreatedByType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    HasBeforeAfter = table.Column<bool>(type: "boolean", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_works", x => x.Id);
                    table.ForeignKey(
                        name: "FK_works_categories_PrimaryCategoryId",
                        column: x => x.PrimaryCategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_works_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_works_CreatedByUserId",
                table: "works",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_works_CreatedByUserId_CreatedAt",
                table: "works",
                columns: new[] { "CreatedByUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_works_PrimaryCategoryId",
                table: "works",
                column: "PrimaryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_works_PublishedAt",
                table: "works",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_works_Status",
                table: "works",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_works_Status_PublishedAt",
                table: "works",
                columns: new[] { "Status", "PublishedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "works");
        }
    }
}
