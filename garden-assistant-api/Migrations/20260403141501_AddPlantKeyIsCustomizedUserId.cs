using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantKeyIsCustomizedUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_customized",
                table: "plants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "key",
                table: "plants",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "plants",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_plants_key",
                table: "plants",
                column: "key",
                unique: true,
                filter: "user_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_plants_user_id",
                table: "plants",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_plants_users_user_id",
                table: "plants",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_plants_users_user_id",
                table: "plants");

            migrationBuilder.DropIndex(
                name: "ix_plants_key",
                table: "plants");

            migrationBuilder.DropIndex(
                name: "ix_plants_user_id",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "is_customized",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "key",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "plants");
        }
    }
}
