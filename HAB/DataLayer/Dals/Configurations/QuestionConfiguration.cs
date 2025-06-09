using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<QuestionDal>
{
    public void Configure(EntityTypeBuilder<QuestionDal> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Text).IsRequired();

        builder.HasOne(q => q.NextQuestion)
            .WithMany()
            .HasForeignKey(q => q.NextQuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}