using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.TBotCommand
{
    public class CreateTelegramBotCommandDto
    {
        [Required]
        [StringLength(50)]
        public string Command { get; set; } = string.Empty;

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Response { get; set; } = string.Empty;

        public int BotSettingsId { get; set; }
    }
}
