using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddGardenAndBedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at_utc",
                table: "plantings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "guild_id",
                table: "plantings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at_utc",
                table: "gardens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_plantings_guild_id",
                table: "plantings",
                column: "guild_id");

            migrationBuilder.AddForeignKey(
                name: "fk_plantings_guilds_guild_id",
                table: "plantings",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_plantings_guilds_guild_id",
                table: "plantings");

            migrationBuilder.DropIndex(
                name: "ix_plantings_guild_id",
                table: "plantings");

            migrationBuilder.DropColumn(
                name: "created_at_utc",
                table: "plantings");

            migrationBuilder.DropColumn(
                name: "guild_id",
                table: "plantings");

            migrationBuilder.DropColumn(
                name: "created_at_utc",
                table: "gardens");
        }
    }
}
