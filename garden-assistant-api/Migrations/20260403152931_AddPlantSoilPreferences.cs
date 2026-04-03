using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantSoilPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "optimal_ph_max",
                table: "plants",
                type: "numeric(3,1)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "optimal_ph_min",
                table: "plants",
                type: "numeric(3,1)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "plant_soil_types",
                columns: table => new
                {
                    plant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    soil_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plant_soil_types", x => new { x.plant_id, x.soil_type });
                    table.ForeignKey(
                        name: "fk_plant_soil_types_plants_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plant_soil_types");

            migrationBuilder.DropColumn(
                name: "optimal_ph_max",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "optimal_ph_min",
                table: "plants");
        }
    }
}
