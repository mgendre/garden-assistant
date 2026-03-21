using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantCalendarEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "frost_sensitive",
                table: "plants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "propagation_method",
                table: "plants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "harvest_readiness",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    days_from_transplant = table.Column<int>(type: "integer", nullable: true),
                    days_from_sowing = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_harvest_readiness", x => x.id);
                    table.ForeignKey(
                        name: "fk_harvest_readiness_plants_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_actions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    half_month_start = table.Column<int>(type: "integer", nullable: false),
                    half_month_end = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plant_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_plant_actions_plants_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "harvest_readiness_criteria",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    harvest_readiness_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criterion_type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_harvest_readiness_criteria", x => x.id);
                    table.ForeignKey(
                        name: "fk_harvest_readiness_criteria_harvest_readiness_harvest_readin",
                        column: x => x.harvest_readiness_id,
                        principalTable: "harvest_readiness",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_harvest_readiness_plant_id",
                table: "harvest_readiness",
                column: "plant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_harvest_readiness_criteria_harvest_readiness_id",
                table: "harvest_readiness_criteria",
                column: "harvest_readiness_id");

            migrationBuilder.CreateIndex(
                name: "ix_plant_actions_plant_id",
                table: "plant_actions",
                column: "plant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "harvest_readiness_criteria");

            migrationBuilder.DropTable(
                name: "plant_actions");

            migrationBuilder.DropTable(
                name: "harvest_readiness");

            migrationBuilder.DropColumn(
                name: "frost_sensitive",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "propagation_method",
                table: "plants");
        }
    }
}
