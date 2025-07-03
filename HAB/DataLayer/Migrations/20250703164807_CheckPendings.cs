using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class CheckPendings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HrUsers",
                columns: new[] { "Id", "Alias", "BotUserId", "Position" },
                values: new object[] { 1, "TECH BOSS", new Guid("d2f8b0c4-3c1e-4b5a-9f6e-7c8d9e0f1a2b"), "Head of Technology" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HrUsers",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
