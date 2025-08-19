using DataLayer.Converters;
using DataLayer.Convertes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<QuestionDal>
{
    public void Configure(EntityTypeBuilder<QuestionDal> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Text).IsRequired();

        builder.HasOne(x => x.Vacancy)
        .WithMany(v => v.Questions)
        .HasForeignKey(x => x.VacancyId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Answer)
            .HasConversion<AnswerTypeValueConverter>();

        // Seed data for questions
        builder.HasData(
            // Software Developer Questions
            new QuestionDal
            {
                Id = new Guid("10000001-0001-0001-0001-000000000001"),
                VacancyId = VacationConfiguration.SoftwareDeveloperVacancyId,
                Text = "Do you have at least 3 years of experience in software development?",
                OrderNumber = 1,
                Answer = new YesNoWithRequiredCorrectAnswerTypeDal 
                { 
                    Expected = true, 
                    UnexpectedAnswerRejectText = "We require at least 3 years of software development experience for this position." 
                }
            },
            new QuestionDal
            {
                Id = new Guid("10000001-0001-0001-0001-000000000002"),
                VacancyId = VacationConfiguration.SoftwareDeveloperVacancyId,
                Text = "Please describe your experience with C# and .NET technologies.",
                OrderNumber = 2,
                Answer = new TextAnswerTypeDal()
            },
            new QuestionDal
            {
                Id = new Guid("10000001-0001-0001-0001-000000000003"),
                VacancyId = VacationConfiguration.SoftwareDeveloperVacancyId,
                Text = "Are you comfortable working in an Agile/Scrum environment?",
                OrderNumber = 3,
                Answer = new YesNoAnswerTypeDal()
            },

            // Data Analyst Questions
            new QuestionDal
            {
                Id = new Guid("20000001-0001-0001-0001-000000000001"),
                VacancyId = VacationConfiguration.DataAnalystVacancyId,
                Text = "Do you have experience with SQL and database querying?",
                OrderNumber = 1,
                Answer = new YesNoWithRequiredCorrectAnswerTypeDal 
                { 
                    Expected = true, 
                    UnexpectedAnswerRejectText = "SQL knowledge is essential for this Data Analyst position." 
                }
            },
            new QuestionDal
            {
                Id = new Guid("20000001-0001-0001-0001-000000000002"),
                VacancyId = VacationConfiguration.DataAnalystVacancyId,
                Text = "Please describe your experience with data visualization tools (e.g., Tableau, Power BI, Python).",
                OrderNumber = 2,
                Answer = new TextAnswerTypeDal()
            },
            new QuestionDal
            {
                Id = new Guid("20000001-0001-0001-0001-000000000003"),
                VacancyId = VacationConfiguration.DataAnalystVacancyId,
                Text = "Have you worked with statistical analysis or machine learning before?",
                OrderNumber = 3,
                Answer = new YesNoAnswerTypeDal()
            },

            // Project Manager Questions
            new QuestionDal
            {
                Id = new Guid("30000001-0001-0001-0001-000000000001"),
                VacancyId = VacationConfiguration.ProjectManagerVacancyId,
                Text = "Do you have at least 2 years of project management experience?",
                OrderNumber = 1,
                Answer = new YesNoWithRequiredCorrectAnswerTypeDal 
                { 
                    Expected = true, 
                    UnexpectedAnswerRejectText = "We require at least 2 years of project management experience for this role." 
                }
            },
            new QuestionDal
            {
                Id = new Guid("30000001-0001-0001-0001-000000000002"),
                VacancyId = VacationConfiguration.ProjectManagerVacancyId,
                Text = "Please describe your experience managing technical teams and software development projects.",
                OrderNumber = 2,
                Answer = new TextAnswerTypeDal()
            },
            new QuestionDal
            {
                Id = new Guid("30000001-0001-0001-0001-000000000003"),
                VacancyId = VacationConfiguration.ProjectManagerVacancyId,
                Text = "Are you familiar with project management methodologies like Agile, Scrum, or Kanban?",
                OrderNumber = 3,
                Answer = new YesNoAnswerTypeDal()
            }
        );
    }
}