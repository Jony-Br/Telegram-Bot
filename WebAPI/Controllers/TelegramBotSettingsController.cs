using DataLayer.Entities;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Dtos;
using System.ComponentModel.DataAnnotations;
using WebAPI.Dtos.TBotSettings;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramBotSettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TelegramBotSettingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/botsettings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BotSettings>>> GetBotSettings()
        {
            var botSettings = await _context.BotSettings
                //.Include(s => s.Commands)
                .Select(s => new TelegramBotSettingsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    WelcomeMessage = s.WelcomeMessage,
                    BotPhotoUrl = s.BotPhotoUrl,
                    BotToken = s.BotToken
                    /*Думаю убрать пока что команды из списка, т.к. они не нужны в этом запросе
                     * 
                    TelegramBotCommand = s.Commands.Select(c => new TelegramBotCommand
                    {
                        Id = c.Id,
                        Command = c.Command,
                        Description = c.Description,
                        Response = c.Response,
                        BotSettingsId= c.BotSettingsId
                    }).ToList()
                    */
                })
                .ToListAsync();

            return Ok(botSettings);
        }

        // Get: api/botsettings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TelegramBotSettingsDto>> GetBotSettings(
        int id)
        {
            var existingBotSettings = await _context.BotSettings
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

            if (existingBotSettings == null)
            {
                return NotFound("Настройки бота з таким 'id' не були знайдені");
            }
            else
            {
                var botSettingsInfo = await _context.BotSettings
                //.Include(s => s.Commands)
                .Select(s => new TelegramBotSettingsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    WelcomeMessage = s.WelcomeMessage,
                    BotPhotoUrl = s.BotPhotoUrl,
                    BotToken = s.BotToken
                    /*
                     * 
                    TelegramBotCommand = s.Commands.Select(c => new TelegramBotCommand
                    {
                        Id = c.Id,
                        Command = c.Command,
                        Description = c.Description,
                        Response = c.Response
                    }).ToList()
                    */
                })
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
                return Ok(botSettingsInfo);
            }

        }


        // GET: api/botsettings/{BotToken}
        [HttpGet("token/{botToken}")]
        public async Task<ActionResult<TelegramBotSettingsDto>> GetBotSettingsByToken(string botToken)
        {
            var existingBotSettings = await _context.BotSettings
                .Where(s => s.BotToken == botToken)
                .FirstOrDefaultAsync();
            if (existingBotSettings == null)
            {
                return NotFound("Настройки бота з таким 'token' не були знайдені \n Прошу додати бота з новим токеном");
            }
            else
            {
                var botSettingsInfo = await _context.BotSettings
                //.Include(s => s.Commands)
                .Select(s => new TelegramBotSettingsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    WelcomeMessage = s.WelcomeMessage,
                    BotPhotoUrl = s.BotPhotoUrl,
                    BotToken = s.BotToken
                })
                .Where(s => s.BotToken == botToken)
                .FirstOrDefaultAsync();
                return Ok(botSettingsInfo);
            }
        }

        // POST: api/botsettings
        [HttpPost]
        public async Task<ActionResult<CreateTelegramBotSettingsDto>> CreateBotSettings(
            [FromBody] CreateTelegramBotSettingsDto dto)
        {
            var existingBotSettings = await _context.BotSettings
                .AnyAsync(s => s.Id == dto.Id);
            if (existingBotSettings)
            {
                return NotFound($"Бот с таким ID - {dto.Id} уже существует");
            }
            else
            {
                var botSettings = new BotSettings
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    WelcomeMessage = dto.WelcomeMessage,
                    BotPhotoUrl = dto.BotPhotoUrl,
                    BotToken = dto.BotToken
                };
                _context.BotSettings.Add(botSettings);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetBotSettings), new { id = botSettings.Id }, botSettings);
            }

        }

        // PUT: api/botsettings/5
        [HttpPut]
        public async Task<ActionResult> UpdateBotSettings([FromBody] UpdateTelegramBotSettingsDto dto)
        {
            var existingBotSettings = await _context.BotSettings
                .Where(s => s.Id == dto.Id)
                .FirstOrDefaultAsync();
            if (existingBotSettings == null)
            {
                return NotFound("Настройки бота з таким 'id' не були знайдені");
            }
            else
            {
                existingBotSettings.Name = dto.Name;
                existingBotSettings.Description = dto.Description;
                existingBotSettings.WelcomeMessage = dto.WelcomeMessage;
                existingBotSettings.BotPhotoUrl = dto.BotPhotoUrl;
                existingBotSettings.BotToken = dto.BotToken;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        // DELETE: api/botsettings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBotSettings(int id)
        {
            var existingBotSettings = await _context.BotSettings
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
            if (existingBotSettings == null)
            {
                return NotFound("Настройки бота з таким 'id' не були знайдені");
            }
            else
            {
                _context.BotSettings.Remove(existingBotSettings);
                await _context.SaveChangesAsync();
                return NoContent();
            }



        }
    }
}
