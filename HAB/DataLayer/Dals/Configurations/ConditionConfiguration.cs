using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class ConditionConfiguration : IEntityTypeConfiguration<ConditionDal>
{
    public void Configure(EntityTypeBuilder<ConditionDal> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Answer).IsRequired();

        builder.HasOne(c => c.Question)
            .WithMany(q => q.Conditions)
            .HasForeignKey(c => c.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}