using DataLayer.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class ApplicationAnswerConfiguration : IEntityTypeConfiguration<ApplicationAnswerDal>
{
    public void Configure(EntityTypeBuilder<ApplicationAnswerDal> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AnswerValue)
            .HasConversion<QuestionAnswerValueConverter>()
            .IsRequired();

        builder.HasOne(a => a.UserApplication)
            .WithMany(u => u.Answers)
            .HasForeignKey(a => a.UserApplicationId);

        builder.HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
