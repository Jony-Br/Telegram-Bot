using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace DataLayer
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext db)
        {
            db.Database.EnsureCreated(); // Создаёт БД, если её нет

            if (db.BotSettings.Any()) return; // Если данные уже есть — выходим

            // Добавляем настройки бота
            /*var botSettings = new BotSettings
            {
                Name = "MyBot",
                Description = "Телеграм-бот для диплома",
                WelcomeMessage = "Привет! Я твой бот.",
                BotPhotoUrl = "https://example.com/default-bot-image.png", // Додаємо дефолтне фото
                Commands = new List<TelegramBotCommand>
            {
                new() { Command = "/start", Description = "Начало работы", Response = "Добро пожаловать!" },
                new() { Command = "/help", Description = "Помощь", Response = "Чем помочь?" }
            }
            };*/

            //db.BotSettings.Add(botSettings);
            db.SaveChanges();
        }
    }
}
