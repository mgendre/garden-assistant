using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToGuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "guilds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_guilds_user_id",
                table: "guilds",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_guilds_users_user_id",
                table: "guilds",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_guilds_users_user_id",
                table: "guilds");

            migrationBuilder.DropIndex(
                name: "ix_guilds_user_id",
                table: "guilds");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "guilds");
        }
    }
}
