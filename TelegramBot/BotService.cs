using DataLayer;
using DataLayer.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Services;

namespace TelegramBot;

public class BotService 
{
    private readonly ITelegramBotClient _botClient;
    //private readonly HttpClient _apiClient;
    private readonly IConfiguration _config;

    private readonly UserService _userService;
    private readonly BotConfigService _configService;

    private readonly ILogger<BotService> _logger;


    public BotService(IConfiguration config, BotConfigService configService, UserService userService, ILogger<BotService> logger)    
    {
        _config = config;
        _botClient = new TelegramBotClient(_config["BotConfiguration:Token"]);
       /* _apiClient = new HttpClient
        {
            BaseAddress = new Uri(_config["BotConfiguration:ApiUrl"])
        };*/
        _userService = userService;
        _configService = configService;
        _logger = logger;
    }



    public async Task Start()
    {
        try
        {
            await _configService.InitializeAsync();

            _logger.LogInformation("Бот успешно инициализирован");
            Console.WriteLine("Бот успешно инициализирован");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка инициализации бота");
            Console.WriteLine("Ошибка инициализации бота" + " ", ex);
        }

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Получаем все типы обновлений
        };

        

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions
        );

        Console.WriteLine("Бот запущен!");
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Telegram.Bot.Types.Update update,
        CancellationToken ct)
    {

        var message = update.Message;

         
        // !!! Нужно подумать над логикой, что бы пользователь не добавлялся в БД каждый раз, когда он отправляет сообщение
        // Обработка добавления клиента к БД или же его обновление 
        await _userService.SyncUserAsync(message);

        // Основная логика обработки сообщений будет здесь
        if (message != null)
        {
            await HandleMessageAsync(botClient, message);
        }
    }

    private Task HandlePollingErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken ct)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(
    ITelegramBotClient botClient,
    Telegram.Bot.Types.Message message)
    {
        Console.WriteLine($"Обрабатываем полученное сообщение: {message.Text}");

        // Обработка команд
        switch (message.Text?.Split(' ')[0])
        {
            case "/start":
                await HandleStartCommand(botClient, message);
                break;

            case "/help":
                await HandleHelpCommand(botClient, message);
                break;

            default:
                await HandleRegularMessage(botClient, message);
                break;
        }
    }

    private async Task HandleStartCommand(
        ITelegramBotClient botClient,
        Telegram.Bot.Types.Message message)
    {
        await botClient.SendMessage(message.Chat.Id, "Полученно сообщение '/start'");

        /*        
        // 1. Получаем welcome-сообщение из API


        var response = await _apiClient.GetAsync("botsettings");
        var content = await response.Content.ReadAsStringAsync();
        

        // 2. Отправляем пользователю
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: content,
            cancellationToken: CancellationToken.None);
        
        */
    }

    private async Task HandleHelpCommand(
        ITelegramBotClient botClient,
        Telegram.Bot.Types.Message message)
    {
        await botClient.SendMessage(message.Chat.Id, "Полученно сообщение '/help'");
        
        /*

         // 1. Получаем список команд из API
         var response = await _apiClient.GetAsync("telegrambotcommands");
         var commands = await response.Content.ReadFromJsonAsync<List<TelegramBotCommand>>();

         // 2. Формируем текст
         var helpText = "Доступные команды:\n" +
             string.Join("\n", commands.Select(c => $"{c.Command} - {c.Description}"));

         // 3. Отправляем
         await botClient.SendMessage(
             chatId: message.Chat.Id,
             text: helpText);

         */
    }


    private async Task HandleRegularMessage(
    ITelegramBotClient botClient,
    Telegram.Bot.Types.Message message)
    {

        await botClient.SendMessage(message.Chat.Id, $"Полученно сообщение {message.Text}");
        /*

        // 1. Проверяем, есть ли шаблон ответа в БД
        var response = await _apiClient.GetAsync(
            $"responsetemplates?keyword={message.Text}");

        if (response.IsSuccessStatusCode)
        {
            // Если нашли шаблон - отправляем
            var template = await response.Content.ReadFromJsonAsync<ResponseTemplate>();
            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: template.Text);
        }
        else
        {
            // Если нет - сохраняем вопрос и просим повторить
            await _apiClient.PostAsJsonAsync("messages", new
            {
                Text = message.Text,
                UserId = message.From.Id,
                IsFromUser = true
            });

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "Я пока не знаю ответа. Попробуйте задать вопрос иначе.");
        }

        */
    }
}
