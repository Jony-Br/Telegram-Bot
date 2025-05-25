using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool? IsFromUser { get; set; } // От пользователя или от бота?
        public int UserId { get; set; } // Внешний ключ
        public UserInfo? User { get; set; }
    }
}
