using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class UserApplicationConfiguration : IEntityTypeConfiguration<UserApplicationDal>
{
    public void Configure(EntityTypeBuilder<UserApplicationDal> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Vacancy)
            .WithMany(v => v.Applications)
            .HasForeignKey(a => a.VacancyId);

        builder.HasOne(a => a.LastQuestion)
            .WithMany()
            .HasForeignKey(a => a.LastQuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}