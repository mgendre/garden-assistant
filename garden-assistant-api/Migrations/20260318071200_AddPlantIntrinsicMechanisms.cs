using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantIntrinsicMechanisms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "allelopathic_risk",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "nitrogen_fixer",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "pollinator_plant",
                table: "plants");

            migrationBuilder.CreateTable(
                name: "plant_intrinsic_mechanisms",
                columns: table => new
                {
                    plant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mechanism = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plant_intrinsic_mechanisms", x => new { x.plant_id, x.mechanism });
                    table.ForeignKey(
                        name: "fk_plant_intrinsic_mechanisms_plants_plant_id",
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
                name: "plant_intrinsic_mechanisms");

            migrationBuilder.AddColumn<bool>(
                name: "allelopathic_risk",
                table: "plants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "nitrogen_fixer",
                table: "plants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "pollinator_plant",
                table: "plants",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
