using DataLayer.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebAPI.Dtos.TBotCommand;
using WebAPI.Dtos.TBotSettings;

namespace TelegramBot.Services
{
    public class BotConfigService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BotConfigService> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IConfiguration _config;

        private TelegramBotSettingsDto _botSettings;
        public int botSettingsId { get; private set; }

        public BotConfigService(HttpClient httpClient, ILogger<BotConfigService> logger, ITelegramBotClient botClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _botClient = botClient;
            _config = config;
        }


        public async Task InitializeAsync()
        {
            try
            {
                var botToken = _config["BotConfiguration:Token"];

                // 1. Проверяем существование бота в БД
                var response = await _httpClient.GetAsync($"/api/TelegramBotSettings/token/{botToken}");

                if (response.IsSuccessStatusCode)
                {
                    // 2. Если бот существует - загружаем настройки
                    _botSettings = await response.Content.ReadFromJsonAsync<TelegramBotSettingsDto>();
                    botSettingsId = _botSettings.Id;

                    _logger.LogInformation("Настройки бота загружены из БД. BotId: {BotId}", botSettingsId);
                }
                else
                {
                    // 3. Если бота нет - регистрируем нового
                    var botInfo = await _botClient.GetMe();

                    var newBot = new CreateTelegramBotSettingsDto
                    {
                        Name = botInfo.Username ?? "Unknown",
                        Description = (await _botClient.GetMyDescription())?.Description ?? string.Empty,
                        WelcomeMessage = string.Empty,
                        BotPhotoUrl = string.Empty,
                        BotToken = botToken
                    };

                    var createResponse = await _httpClient.PostAsJsonAsync("api/BotSettings", newBot);

                    if (!createResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"Ошибка регистрации бота: {createResponse.ReasonPhrase}");
                    }

                    _botSettings = await createResponse.Content.ReadFromJsonAsync<TelegramBotSettingsDto>();
                    botSettingsId = _botSettings.Id;

                    _logger.LogInformation("Бот успешно зарегистрирован в БД. BotId: {BotId}", botSettingsId);
                }

                // 4. Загружаем и устанавливаем команды
                await LoadAndSetCommandsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка инициализации бота");
                throw; // Пробрасываем исключение для остановки приложения
            }
        }

        private async Task LoadAndSetCommandsAsync()
        {
            try
            {
                var commands = await _httpClient.GetFromJsonAsync<List<BotCommand>>($"api/TelegramBotCommand/by-botSettingsId/{botSettingsId}");

                if (commands?.Any() == true)
                {
                    await _botClient.SetMyCommands(commands);
                    _logger.LogInformation("Установлено {Count} команд бота", commands.Count);
                }
                else
                {
                    _logger.LogWarning("Для бота не найдено команд в БД");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка загрузки команд бота");
                throw;
            }
        }

        public string GetBotName() => _botSettings?.Name ?? "Unknown";
        public string GetWelcomeMessage() => _botSettings?.WelcomeMessage ?? "Добро пожаловать!";
    }
}

