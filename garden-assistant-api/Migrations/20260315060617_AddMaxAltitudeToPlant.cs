using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxAltitudeToPlant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "max_altitude_m",
                table: "plants",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_altitude_m",
                table: "plants");
        }
    }
}
