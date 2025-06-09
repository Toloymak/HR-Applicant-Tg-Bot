using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class BotUserConfiguration : IEntityTypeConfiguration<BotUserDal>
{
    public void Configure(EntityTypeBuilder<BotUserDal> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.TgName).IsRequired();

        builder.HasData(new BotUserDal
        {
            TgId = "319556101",
            TgName = "brovko_a",
            CustomName = "God admin"
        });
    }
}