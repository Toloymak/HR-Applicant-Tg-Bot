using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class VacationConfiguration : IEntityTypeConfiguration<VacancyDal>
{
    // Predefined vacancy IDs for consistency
    public static readonly Guid SoftwareDeveloperVacancyId = new("11111111-1111-1111-1111-111111111111");
    public static readonly Guid DataAnalystVacancyId = new("22222222-2222-2222-2222-222222222222");
    public static readonly Guid ProjectManagerVacancyId = new("33333333-3333-3333-3333-333333333333");
    
    public void Configure(EntityTypeBuilder<VacancyDal> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.DefaultRejectText).IsRequired();
        builder.Property(v => v.FinishedApplicationText).IsRequired();
        builder.Property(v => v.IsActive).IsRequired();
        
        builder.HasOne(x => x.Hr)
            .WithMany()
            .HasForeignKey(x => x.HrId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(x => x.Questions)
            .WithOne()
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data for vacancies
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        builder.HasData(
            new VacancyDal
            {
                Id = SoftwareDeveloperVacancyId,
                HrId = null, // Will be assigned later when HR users are created
                Title = "Senior Software Developer",
                Description = "We are looking for an experienced software developer to join our team. You will work on cutting-edge projects using modern technologies and contribute to building scalable applications.",
                IsArchived = false,
                IsActive = true,
                DefaultRejectText = "Thank you for your interest in the Senior Software Developer position. Unfortunately, we have decided to move forward with other candidates at this time. We wish you the best in your job search!",
                FinishedApplicationText = "🎉 Congratulations! You have successfully completed your application for the Senior Software Developer position. Our HR team will review your responses and get back to you within 5-7 business days. Thank you for your interest in joining our team!",
                CreatedAt = seedDate
            },
            new VacancyDal
            {
                Id = DataAnalystVacancyId,
                HrId = null,
                Title = "Data Analyst",
                Description = "Join our data team as a Data Analyst! You'll work with large datasets, create insightful reports, and help drive data-driven decision making across the organization.",
                IsArchived = false,
                IsActive = true,
                DefaultRejectText = "Thank you for applying to the Data Analyst position. After careful consideration, we have decided to proceed with other candidates. We appreciate your time and interest in our company.",
                FinishedApplicationText = "✅ Your application for the Data Analyst position has been submitted successfully! We're excited to learn more about your background. Our team will review your application and contact you within 3-5 business days.",
                CreatedAt = seedDate
            },
            new VacancyDal
            {
                Id = ProjectManagerVacancyId,
                HrId = null,
                Title = "Technical Project Manager",
                Description = "We're seeking a Technical Project Manager to lead cross-functional teams and deliver complex software projects. You'll coordinate with developers, designers, and stakeholders to ensure successful project delivery.",
                IsArchived = false,
                IsActive = true,
                DefaultRejectText = "Thank you for your application for the Technical Project Manager role. While your background is impressive, we have chosen to move forward with candidates whose experience more closely aligns with our current needs.",
                FinishedApplicationText = "🚀 Thank you for completing your application for the Technical Project Manager position! We're impressed by your interest in leading our technical initiatives. Expect to hear from us within one week regarding next steps.",
                CreatedAt = seedDate
            }
        );
    }
}