using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshowcaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryCategoryIdToProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryCategoryId",
                table: "professional_profiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_professional_profiles_PrimaryCategoryId",
                table: "professional_profiles",
                column: "PrimaryCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_professional_profiles_categories_PrimaryCategoryId",
                table: "professional_profiles",
                column: "PrimaryCategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                UPDATE professional_profiles p
                SET ""PrimaryCategoryId"" = c.""Id""
                FROM categories c
                WHERE lower(trim(p.""PrimaryCategory"")) = c.""NormalizedName"";
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM professional_profiles WHERE ""PrimaryCategoryId"" IS NULL
                    ) THEN
                        RAISE NOTICE 'Some profiles have no matching category - setting PrimaryCategoryId to first active category';
                        UPDATE professional_profiles
                        SET ""PrimaryCategoryId"" = (SELECT ""Id"" FROM categories WHERE ""IsActive"" = true LIMIT 1)
                        WHERE ""PrimaryCategoryId"" IS NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrimaryCategoryId",
                table: "professional_profiles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "PrimaryCategory",
                table: "professional_profiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryCategory",
                table: "professional_profiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrimaryCategoryId",
                table: "professional_profiles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.DropForeignKey(
                name: "FK_professional_profiles_categories_PrimaryCategoryId",
                table: "professional_profiles");

            migrationBuilder.DropIndex(
                name: "IX_professional_profiles_PrimaryCategoryId",
                table: "professional_profiles");

            migrationBuilder.DropColumn(
                name: "PrimaryCategoryId",
                table: "professional_profiles");
        }
    }
}