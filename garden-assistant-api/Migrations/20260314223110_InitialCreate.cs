using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    scientific_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    family = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    genus = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    life_cycle = table.Column<int>(type: "integer", nullable: false),
                    height_at_maturity_cm = table.Column<int>(type: "integer", nullable: true),
                    root_depth = table.Column<int>(type: "integer", nullable: false),
                    sun_requirement = table.Column<int>(type: "integer", nullable: false),
                    water_needs = table.Column<int>(type: "integer", nullable: false),
                    nitrogen_fixer = table.Column<bool>(type: "boolean", nullable: false),
                    allelopathic_risk = table.Column<bool>(type: "boolean", nullable: false),
                    pollinator_plant = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plant_associations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_plant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_plant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mechanism = table.Column<int>(type: "integer", nullable: false),
                    effect = table.Column<int>(type: "integer", nullable: false),
                    distance_effect = table.Column<int>(type: "integer", nullable: false),
                    confidence_level = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plant_associations", x => x.id);
                    table.ForeignKey(
                        name: "fk_plant_associations_plants_source_plant_id",
                        column: x => x.source_plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plant_associations_plants_target_plant_id",
                        column: x => x.target_plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gardens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gardens", x => x.id);
                    table.ForeignKey(
                        name: "fk_gardens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plantings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    garden_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    planned_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plantings", x => x.id);
                    table.ForeignKey(
                        name: "fk_plantings_gardens_garden_id",
                        column: x => x.garden_id,
                        principalTable: "gardens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plantings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "planting_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    planting_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: true),
                    position_x = table.Column<float>(type: "real", nullable: true),
                    position_y = table.Column<float>(type: "real", nullable: true),
                    layer = table.Column<int>(type: "integer", nullable: true),
                    planned_sow_date = table.Column<DateOnly>(type: "date", nullable: true),
                    planned_harvest_date = table.Column<DateOnly>(type: "date", nullable: true),
                    actual_harvest_date = table.Column<DateOnly>(type: "date", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_planting_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_planting_entries_plantings_planting_id",
                        column: x => x.planting_id,
                        principalTable: "plantings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_planting_entries_plants_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "admin@gardenassistant.local" });

            migrationBuilder.CreateIndex(
                name: "ix_gardens_user_id",
                table: "gardens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_plant_associations_source_plant_id_target_plant_id_mechanism",
                table: "plant_associations",
                columns: new[] { "source_plant_id", "target_plant_id", "mechanism" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_plant_associations_target_plant_id",
                table: "plant_associations",
                column: "target_plant_id");

            migrationBuilder.CreateIndex(
                name: "ix_planting_entries_plant_id",
                table: "planting_entries",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "ix_planting_entries_planting_id",
                table: "planting_entries",
                column: "planting_id");

            migrationBuilder.CreateIndex(
                name: "ix_plantings_garden_id",
                table: "plantings",
                column: "garden_id");

            migrationBuilder.CreateIndex(
                name: "ix_plantings_user_id",
                table: "plantings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plant_associations");

            migrationBuilder.DropTable(
                name: "planting_entries");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "plantings");

            migrationBuilder.DropTable(
                name: "plants");

            migrationBuilder.DropTable(
                name: "gardens");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
