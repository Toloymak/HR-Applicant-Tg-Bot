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
    }
}