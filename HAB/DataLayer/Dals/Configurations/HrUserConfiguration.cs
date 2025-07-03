using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class HrUserConfiguration :  IEntityTypeConfiguration<HrUserDal>
{
    public void Configure(EntityTypeBuilder<HrUserDal> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(x => x.Id).HasSentinel(1);
        
        builder.Property(u => u.Alias).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Position).IsRequired().HasMaxLength(100);
        
        builder.HasOne(u => u.BotUser)
            .WithOne()
            .HasForeignKey<HrUserDal>(u => u.BotUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}