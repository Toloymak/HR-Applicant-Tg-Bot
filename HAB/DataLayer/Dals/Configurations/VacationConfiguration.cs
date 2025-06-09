using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class VacationConfiguration : IEntityTypeConfiguration<VacancyDal>
{
    public void Configure(EntityTypeBuilder<VacancyDal> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.DefaultRejectText).IsRequired();
        builder.Property(v => v.FinishedApplicationText).IsRequired();

        builder.HasOne(v => v.RootQuestion)
            .WithMany()
            .HasForeignKey(v => v.RootQuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}