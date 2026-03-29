using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshowcaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkProfessionals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_professionals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfessionalUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProfessionCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Whatsapp = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_professionals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_work_professionals_users_ProfessionalUserId",
                        column: x => x.ProfessionalUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_professionals_works_WorkId",
                        column: x => x.WorkId,
                        principalTable: "works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_work_professionals_ProfessionalUserId",
                table: "work_professionals",
                column: "ProfessionalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_work_professionals_WorkId",
                table: "work_professionals",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_work_professionals_WorkId_SortOrder",
                table: "work_professionals",
                columns: new[] { "WorkId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_professionals");
        }
    }
}
