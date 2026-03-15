using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddGuildTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guilds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guild_plants",
                columns: table => new
                {
                    guild_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guild_plants", x => new { x.guild_id, x.plant_id });
                    table.ForeignKey(
                        name: "fk_guild_plants_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_guild_plants_plants_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_guild_plants_plant_id",
                table: "guild_plants",
                column: "plant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_plants");

            migrationBuilder.DropTable(
                name: "guilds");
        }
    }
}
