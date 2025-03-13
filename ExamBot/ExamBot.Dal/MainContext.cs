using ExamBot.Dal.Entities;
using ExamBot.Dal.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ExamBot.Dal;

public class MainContext : DbContext
{
    public DbSet<BotUser> Users { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=WIN-MFTSEJNVJT0\\SQLEXPRESS;Database=ExamBot;User Id=sa;Password=1;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
    }

}
