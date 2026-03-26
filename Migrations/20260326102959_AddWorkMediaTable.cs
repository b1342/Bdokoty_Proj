using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshowcaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkMediaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaType = table.Column<string>(type: "text", nullable: false),
                    MediaUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ExternalMediaId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsCover = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_work_media_works_WorkId",
                        column: x => x.WorkId,
                        principalTable: "works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_work_media_WorkId",
                table: "work_media",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_work_media_WorkId_SortOrder",
                table: "work_media",
                columns: new[] { "WorkId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_media");
        }
    }
}
