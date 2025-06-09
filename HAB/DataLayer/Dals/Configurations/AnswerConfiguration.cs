using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<AnswerDal>
{
    public void Configure(EntityTypeBuilder<AnswerDal> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AnswerText).IsRequired(false);

        builder.HasOne(a => a.UserApplication)
            .WithMany(u => u.Answers)
            .HasForeignKey(a => a.UserApplicationId);

        builder.HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}