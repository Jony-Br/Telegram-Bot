using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace DataLayer
{
    
    public class AppDbContext : DbContext
    {
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<BotSettings> BotSettings { get; set; }
        public DbSet<TelegramBotCommand> BotCommands { get; set; }
        public DbSet<ResponseTemplate> ResponseTemplates { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql("Host=localhost;Database=telegram_bot_db;Username=bot_user;Password=telegram_bot");
    }

}

