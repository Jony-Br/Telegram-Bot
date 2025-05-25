using System.ComponentModel.DataAnnotations;
using DataLayer.Entities;

namespace WebAPI.Dtos.TBotSettings
{
    public class TelegramBotSettingsDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength (50)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string BotPhotoUrl { get; set; } = string.Empty; // Ссылка на аватар
        [Required]
        public string BotToken { get; set; } = string.Empty;
        //public List<TelegramBotCommand>? TelegramBotCommand { get; set; } = new(); // Список команд

    }
}
