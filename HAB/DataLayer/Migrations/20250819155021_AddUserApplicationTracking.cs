using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserApplicationTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BotUserId",
                table: "UserApplications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivity",
                table: "UserApplications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "UserApplications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_BotUserId",
                table: "UserApplications",
                column: "BotUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_LastActivity",
                table: "UserApplications",
                column: "LastActivity");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_StartDate",
                table: "UserApplications",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplications_State",
                table: "UserApplications",
                column: "State");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_BotUsers_BotUserId",
                table: "UserApplications",
                column: "BotUserId",
                principalTable: "BotUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_BotUsers_BotUserId",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_BotUserId",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_LastActivity",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_StartDate",
                table: "UserApplications");

            migrationBuilder.DropIndex(
                name: "IX_UserApplications_State",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "BotUserId",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "LastActivity",
                table: "UserApplications");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "UserApplications");
        }
    }
}
