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
        builder.Property(v => v.IsActive).IsRequired();
        
        builder.HasOne(x => x.Hr)
            .WithMany()
            .HasForeignKey(x => x.HrId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(x => x.Questions)
            .WithOne()
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}