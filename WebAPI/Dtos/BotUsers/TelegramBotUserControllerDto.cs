using DataLayer.Entities;

namespace WebAPI.Dtos.BotUsers
{
    public class TelegramBotUserControllerDto
    {
        public int Id { get; set; }
        public required string TelegramUserId { get; set; } // ID в Telegram (string, так как может быть большим)
        public string Username { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
