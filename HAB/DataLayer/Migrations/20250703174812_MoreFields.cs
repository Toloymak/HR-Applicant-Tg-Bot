using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class MoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_Vacations_VacancyId",
                table: "UserApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacations_Questions_RootQuestionId",
                table: "Vacations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vacations",
                table: "Vacations");

            migrationBuilder.RenameTable(
                name: "Vacations",
                newName: "Vacancies");

            migrationBuilder.RenameIndex(
                name: "IX_Vacations_RootQuestionId",
                table: "Vacancies",
                newName: "IX_Vacancies_RootQuestionId");

            migrationBuilder.DropColumn(
                name: "HrId",
                table: "Vacancies");
            
            migrationBuilder.AddColumn<int>(
                name: "HrId",
                table: "Vacancies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Vacancies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vacancies",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Vacancies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Vacancies",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vacancies",
                table: "Vacancies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_HrId",
                table: "Vacancies",
                column: "HrId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_Vacancies_VacancyId",
                table: "UserApplications",
                column: "VacancyId",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_HrUsers_HrId",
                table: "Vacancies",
                column: "HrId",
                principalTable: "HrUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Questions_RootQuestionId",
                table: "Vacancies",
                column: "RootQuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApplications_Vacancies_VacancyId",
                table: "UserApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_HrUsers_HrId",
                table: "Vacancies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Questions_RootQuestionId",
                table: "Vacancies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vacancies",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_HrId",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Vacancies");

            migrationBuilder.RenameTable(
                name: "Vacancies",
                newName: "Vacations");

            migrationBuilder.RenameIndex(
                name: "IX_Vacancies_RootQuestionId",
                table: "Vacations",
                newName: "IX_Vacations_RootQuestionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "HrId",
                table: "Vacations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vacations",
                table: "Vacations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplications_Vacations_VacancyId",
                table: "UserApplications",
                column: "VacancyId",
                principalTable: "Vacations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacations_Questions_RootQuestionId",
                table: "Vacations",
                column: "RootQuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
