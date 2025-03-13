using ExamBot.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamBot.Dal.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<BotUser>
{
    public void Configure(EntityTypeBuilder<BotUser> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.BotUserId);

        builder.HasIndex(u => u.BotUserId).IsUnique();

        builder.HasIndex(u => u.TelegramUserId).IsUnique();

        builder.HasOne(bu => bu.UserInfo)
            .WithOne(ui => ui.BotUserr)
            .HasForeignKey<UserInfo>(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}