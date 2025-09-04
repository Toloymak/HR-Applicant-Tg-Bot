using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Dals.Configurations;

public class UserApplicationConfiguration : IEntityTypeConfiguration<UserApplicationDal>
{
    public void Configure(EntityTypeBuilder<UserApplicationDal> builder)
    {
        builder.HasKey(a => a.Id);

        // Relationship with BotUser
        builder.HasOne(a => a.BotUser)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.BotUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Vacancy)
            .WithMany(v => v.Applications)
            .HasForeignKey(a => a.VacancyId);

        builder.HasOne(a => a.LastQuestion)
            .WithMany()
            .HasForeignKey(a => a.LastQuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Required date fields
        builder.Property(a => a.StartDate).IsRequired();
        builder.Property(a => a.LastActivity).IsRequired();

        // ChatId is required for sending messages to users
        builder.Property(a => a.ChatId).IsRequired();

        // Indexes for performance
        builder.HasIndex(a => a.BotUserId);
        builder.HasIndex(a => a.State);
        builder.HasIndex(a => a.StartDate);
        builder.HasIndex(a => a.LastActivity);
        builder.HasIndex(a => a.ChatId);
    }
}