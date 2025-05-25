using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TelegramBot;
using TelegramBot.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // 1. Конфигурация
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection("BotConfiguration"));

        // 2. Регистрация Telegram Bot Client
        services.AddSingleton<ITelegramBotClient>(sp =>
                    new TelegramBotClient(sp.GetRequiredService<IConfiguration>()["BotConfiguration:Token"]));


        // 3. HTTP-клиенты
        var apiUrl = context.Configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("'ApiSettings:BaseUrl' not configured");

        if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException($"Invalid API URL format: {apiUrl}");
        }

        // 4. Регистрация сервисов
        services.AddHttpClient<BotConfigService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7199");
        });

        services.AddHttpClient<UserService>(client =>
        {
            client.BaseAddress = baseUri;
        });

       
        services.AddSingleton<BotService>();

        
    })
    .Build();

// Запуск бота
try
{
    var bot = builder.Services.GetRequiredService<BotService>();
    bot.Start();

    Console.WriteLine("Бот успешно запущен");
    await builder.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка запуска бота: {ex.Message}");
}