using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class BotUserConfiguration : IEntityTypeConfiguration<BotUserDal>
{
    public static Guid DefaultAdminId = new("d2f8b0c4-3c1e-4b5a-9f6e-7c8d9e0f1a2b");
    
    public void Configure(EntityTypeBuilder<BotUserDal> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.TgName).IsRequired();

        builder.HasData(new BotUserDal
        {
            Id = DefaultAdminId,
            TgId = "319556101",
            TgName = "brovko_a",
        });
    }
}