using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class ResponseTemplate
    {
        public int Id { get; set; }
        public string Keyword { get; set; } = string.Empty; // Ключевое слово для триггера
        public string Text { get; set; } = string.Empty;// Текст ответа
    }
}
