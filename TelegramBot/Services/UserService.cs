/*using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Dtos.BotUsers;

namespace TelegramBot.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SyncUserAsync(Telegram.Bot.Types.Message message)
        {
            

            var userDto = new CreateTelegramBotUserControllerDto
            {
                TelegramUserId = message.From.Id.ToString(),
                Username = message.From.Username,
                FirstName = message.From.FirstName,
                LastName = message.From.LastName
            };

            var response = await _httpClient.PostAsJsonAsync("api/users", userDto);
            response.EnsureSuccessStatusCode();
        }
    }
}*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Dtos.BotUsers;
using DataLayer.Entities;
using Microsoft.Extensions.Logging;
using System.Net;

namespace TelegramBot.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        public UserService(HttpClient httpClient, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SyncUserAsync(Telegram.Bot.Types.Message message)
        {

            try
            {
                var telegramUserId = message.From.Id.ToString();
                var existUser = await GetUserByTelegramIdAsync(telegramUserId);

                var UserDto = new TelegramBotUserControllerDto
                {
                    TelegramUserId = telegramUserId,
                    Username = message.From.Username,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                };

                if (existUser == null)
                {

                    //Создание нового пользователя
                    var createUserDto = new CreateTelegramBotUserControllerDto
                    {
                        TelegramUserId = telegramUserId,
                        Username = message.From.Username,
                        FirstName = message.From.FirstName,
                        LastName = message.From.LastName,
                    };

                    var response = await _httpClient.PostAsJsonAsync("api/TelegramUsers", createUserDto);
                    response.EnsureSuccessStatusCode();
                    _logger.LogInformation($"User {telegramUserId} created successfully");

                }
                else
                {
                    var updateUserDto = new UpdateTelegramBotUserControllerDto
                    {
                        Id = existUser.Id,
                        TelegramUserId = telegramUserId,
                        Username = message.From.Username,
                        FirstName = message.From.FirstName,
                        LastName = message.From.LastName,
                    };


                    // Проверяем, нужно ли обновлять пользователя
                    bool needsUpdate = existUser.Username != updateUserDto.Username ||
                                   existUser.FirstName != updateUserDto.FirstName ||
                                   existUser.LastName != updateUserDto.LastName;
                    if (needsUpdate)
                    {
                        var response = await _httpClient.PutAsJsonAsync($"api/TelegramUsers/{existUser.Id}", updateUserDto);
                        response.EnsureSuccessStatusCode();
                        _logger.LogInformation($"User {telegramUserId} updated successfully");
                    }

                    _logger.LogInformation($"User {telegramUserId} already exists");
                    Console.WriteLine($"User {telegramUserId} already exists, no update needed");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error while syncing user: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while syncing user");
                throw;
            }

        }

        public async Task<UserInfo?> GetUserByTelegramIdAsync(string telegramUserId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/TelegramUsers/by-telegram-id/{telegramUserId}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation($"User {telegramUserId} not found");
                }
                // Логируем сырой ответ для отладки
                var json = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"API response: {json}");

                Console.WriteLine($"API response: {response}");
                // Проверяем, успешен ли ответ
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserInfo>();
                }

                

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {telegramUserId}");
                return null;
            }
        }
    }
}
