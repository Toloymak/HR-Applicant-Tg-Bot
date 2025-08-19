using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddVacancySeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Vacancies",
                columns: new[] { "Id", "CreatedAt", "DefaultRejectText", "Description", "FinishedApplicationText", "HrId", "IsActive", "IsArchived", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thank you for your interest in the Senior Software Developer position. Unfortunately, we have decided to move forward with other candidates at this time. We wish you the best in your job search!", "We are looking for an experienced software developer to join our team. You will work on cutting-edge projects using modern technologies and contribute to building scalable applications.", "🎉 Congratulations! You have successfully completed your application for the Senior Software Developer position. Our HR team will review your responses and get back to you within 5-7 business days. Thank you for your interest in joining our team!", null, true, false, "Senior Software Developer" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thank you for applying to the Data Analyst position. After careful consideration, we have decided to proceed with other candidates. We appreciate your time and interest in our company.", "Join our data team as a Data Analyst! You'll work with large datasets, create insightful reports, and help drive data-driven decision making across the organization.", "✅ Your application for the Data Analyst position has been submitted successfully! We're excited to learn more about your background. Our team will review your application and contact you within 3-5 business days.", null, true, false, "Data Analyst" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thank you for your application for the Technical Project Manager role. While your background is impressive, we have chosen to move forward with candidates whose experience more closely aligns with our current needs.", "We're seeking a Technical Project Manager to lead cross-functional teams and deliver complex software projects. You'll coordinate with developers, designers, and stakeholders to ensure successful project delivery.", "🚀 Thank you for completing your application for the Technical Project Manager position! We're impressed by your interest in leading our technical initiatives. Expect to hear from us within one week regarding next steps.", null, true, false, "Technical Project Manager" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Answer", "OrderNumber", "Text", "VacancyId", "VacancyId1" },
                values: new object[,]
                {
                    { new Guid("10000001-0001-0001-0001-000000000001"), "{\"Type\":\"yesno_required\",\"Expected\":true,\"UnexpectedAnswerRejectText\":\"We require at least 3 years of software development experience for this position.\"}", 1, "Do you have at least 3 years of experience in software development?", new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("10000001-0001-0001-0001-000000000002"), "{\"Type\":\"text\"}", 2, "Please describe your experience with C# and .NET technologies.", new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("10000001-0001-0001-0001-000000000003"), "{\"Type\":\"yesno\"}", 3, "Are you comfortable working in an Agile/Scrum environment?", new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("20000001-0001-0001-0001-000000000001"), "{\"Type\":\"yesno_required\",\"Expected\":true,\"UnexpectedAnswerRejectText\":\"SQL knowledge is essential for this Data Analyst position.\"}", 1, "Do you have experience with SQL and database querying?", new Guid("22222222-2222-2222-2222-222222222222"), null },
                    { new Guid("20000001-0001-0001-0001-000000000002"), "{\"Type\":\"text\"}", 2, "Please describe your experience with data visualization tools (e.g., Tableau, Power BI, Python).", new Guid("22222222-2222-2222-2222-222222222222"), null },
                    { new Guid("20000001-0001-0001-0001-000000000003"), "{\"Type\":\"yesno\"}", 3, "Have you worked with statistical analysis or machine learning before?", new Guid("22222222-2222-2222-2222-222222222222"), null },
                    { new Guid("30000001-0001-0001-0001-000000000001"), "{\"Type\":\"yesno_required\",\"Expected\":true,\"UnexpectedAnswerRejectText\":\"We require at least 2 years of project management experience for this role.\"}", 1, "Do you have at least 2 years of project management experience?", new Guid("33333333-3333-3333-3333-333333333333"), null },
                    { new Guid("30000001-0001-0001-0001-000000000002"), "{\"Type\":\"text\"}", 2, "Please describe your experience managing technical teams and software development projects.", new Guid("33333333-3333-3333-3333-333333333333"), null },
                    { new Guid("30000001-0001-0001-0001-000000000003"), "{\"Type\":\"yesno\"}", 3, "Are you familiar with project management methodologies like Agile, Scrum, or Kanban?", new Guid("33333333-3333-3333-3333-333333333333"), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("10000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("10000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("10000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("20000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("20000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("20000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("30000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Vacancies",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Vacancies",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Vacancies",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }
    }
}
