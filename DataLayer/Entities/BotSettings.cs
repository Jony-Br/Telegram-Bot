using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class BotSettings
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string BotPhotoUrl { get; set; } = string.Empty; // Ссылка на аватар
        public string BotToken { get; set; } = string.Empty;
        //public List<TelegramBotCommand> Commands { get; set; } = new(); // Список команд
    }
}
