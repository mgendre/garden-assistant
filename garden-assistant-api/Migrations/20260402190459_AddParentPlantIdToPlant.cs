using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddParentPlantIdToPlant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_plant_id",
                table: "plants",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_plants_parent_plant_id",
                table: "plants",
                column: "parent_plant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_plants_plants_parent_plant_id",
                table: "plants",
                column: "parent_plant_id",
                principalTable: "plants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_plants_plants_parent_plant_id",
                table: "plants");

            migrationBuilder.DropIndex(
                name: "ix_plants_parent_plant_id",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "parent_plant_id",
                table: "plants");
        }
    }
}
