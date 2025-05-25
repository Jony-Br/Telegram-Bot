using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class TelegramBotCommand
    {
        public int Id { get; set; }
        public string Command { get; set; } = string.Empty; // Например, "/start"
        public string Description { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty; // Ответ бота
        public int BotSettingsId { get; set; } = 0; // Внешний ключ
        public BotSettings? BotSettings { get; set; }
    }
}
