using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class BotUserConfiguration : IEntityTypeConfiguration<BotUserDal>
{
    public static Guid DefaultAdminId = new("d2f8b0c4-3c1e-4b5a-9f6e-7c8d9e0f1a2b");
    
    public void Configure(EntityTypeBuilder<BotUserDal> builder)
    {
        builder.HasKey(u => u.Id);
        
        // TgId should be unique when not null (for actual Telegram users)
        builder.HasIndex(u => u.TgId).IsUnique().HasFilter("\"TgId\" IS NOT NULL");
        
        // TgName is optional (user might not have a username)
        builder.Property(u => u.TgName).HasMaxLength(32);
        
        // LastActivity and CreatedAt are required
        builder.Property(u => u.LastActivity).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();

        var defaultCreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        builder.HasData(new BotUserDal
        {
            Id = DefaultAdminId,
            TgId = 319556101,
            TgName = "brovko_a",
            LastActivity = defaultCreatedAt,
            CreatedAt = defaultCreatedAt,
        });
    }
}