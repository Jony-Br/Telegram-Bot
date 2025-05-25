using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebAPI.Dtos;
using WebAPI.Dtos.TBotCommand;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramBotCommandController : ControllerBase
    {
        /* private readonly AppDbContext _context;

         public TelegramBotCommandController(AppDbContext context)
         {
             _context = context;
         }

         // GET: api/botcommands
         [HttpGet]
         public async Task<ActionResult<IEnumerable<TelegramBotCommand>>> GetBotCommands()
         {
             var commands = await _context.BotCommands
                 .Select(c => new TelegramBotCommand
                 {
                     Id = c.Id,
                     Command = c.Command,
                     Description = c.Description,
                     Response = c.Response,
                     BotSettingsId = c.BotSettingsId,
                     BotSettings = c.BotSettings
                 })
                 .ToListAsync();

             return commands;
         }
         // GET: api/botcommands{botid}
         [HttpGet("by-bot/{botId}")]
         public async Task<ActionResult<IEnumerable<TelegramBotCommand>>> GetCommandsByBot(int botId)
         {
             var commands = await _context.BotCommands
                 .Where(u => u.BotSettingsId == botId)
                 .Select(c => new TelegramBotCommand
                 {
                     Id = c.Id,
                     Command = c.Command,
                     Description = c.Description,
                     Response = c.Response,
                     BotSettingsId = c.BotSettingsId,
                     BotSettings = c.BotSettings
                 })
                 .ToListAsync();

             if (!commands.Any())
             {
                 return NotFound();
             }

             return commands;
         }

         // POST : api/addbotcommand
         [HttpPost]
         public async Task<ActionResult<IEnumerable<TelegramBotCommand>>> CreateCommand(
             [FromBody] CreateTelegramBotCommandDto dto)
         {
             var commands = await _context.BotCommands;

         }
 */

        private readonly AppDbContext _context;

        public TelegramBotCommandController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/telegrambotcommands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TelegramBotCommand>>> GetCommands()
        {
            var commands = await _context.BotCommands
                .Include(c => c.BotSettings)
                .Select(c => new TelegramBotCommandDto
                {
                    Id = c.Id,
                    Command = c.Command,
                    Description = c.Description,
                    Response = c.Response,
                    BotSettingsId = c.BotSettingsId
                })
                .ToListAsync();

            return Ok(commands);
        }

        // GET: api/telegrambotcommands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TelegramBotCommand>> GetCommand(int id)
        {
            var command = await _context.BotCommands.FindAsync(id);

            if (command == null)
            {
                return NotFound("Команда з таким 'id' не був знайдений");
            }
            else
            {

                var result = await _context.BotCommands.
                    Include(c => c.BotSettings)
                    .Select(c => new TelegramBotCommandDto
                    {
                        Id = c.Id,
                        Command = c.Command,
                        Description = c.Description,
                        Response = c.Response,
                        BotSettingsId = c.BotSettingsId
                    })
                    .Where(c => c.Id == command.Id)
                    .FirstAsync();
                return Ok(result);
            }

        }


        // GET: api/telegrambotcommands/by-bot/{BotSettingsId}
        [HttpGet("by-botSettingsId/{botSettingsId}")]
        public async Task<ActionResult<IEnumerable<TelegramBotCommand>>> GetCommandsByBotSettingsId(int botSettingsId)
        {
            var commands = await _context.BotCommands
                .Where(c => c.BotSettingsId == botSettingsId)
                .Select(c => new TelegramBotCommandDto
                {
                    Id = c.Id,
                    Command = c.Command,
                    Description = c.Description,
                    Response = c.Response,
                    BotSettingsId = c.BotSettingsId
                })
                .ToListAsync();
            if (!commands.Any())
            {
                return NotFound("Команди для цього бота не знайдені");
            }
            return Ok(commands);
        }

        // POST: api/telegrambotcommands

        // !! Нужно добавить валидацию которые будут соответствовать правилам телеграм бота, что бы потом небыло проблем 
        // Валидация команд:
        [HttpPost]
        public async Task<ActionResult<TelegramBotCommand>> CreateCommand(
            [FromBody] CreateTelegramBotCommandDto dto)
        {

            // Проверяем, существует ли BotSettings с таким Id
            var botSettingsExists = await _context.BotSettings.AnyAsync(b => b.Id == dto.BotSettingsId);
            if (!botSettingsExists && dto.BotSettingsId != 0) // если 0 — разрешаем (опционально)
            {
                return BadRequest("BotSettings with the specified ID does not exist.");
            }

            var newCommand = new TelegramBotCommand
            {
                Command = dto.Command,
                Description = dto.Description,
                Response = dto.Response,
                BotSettingsId = dto.BotSettingsId
            };


            var commandExists = await _context.BotCommands
                .AnyAsync(c => c.Command == newCommand.Command && c.BotSettingsId == newCommand.BotSettingsId);
            if (commandExists)
            {
                return BadRequest("Команда вже існує у цьому боті");
            }
            else
            {
                _context.BotCommands.Add(newCommand);
                await _context.SaveChangesAsync();
            }


            var responseDto = new TelegramBotCommand
            {
                Id = newCommand.Id,
                Command = newCommand.Command,
                Description = newCommand.Description,
                Response = newCommand.Response,
                BotSettingsId = newCommand.BotSettingsId
            };

            return CreatedAtAction(nameof(GetCommand),
                new { id = newCommand.Id },
                responseDto);
        }

        // PUT: api/telegrambotcommands/5
        [HttpPut]
        public async Task<IActionResult> UpdateCommand([FromBody] UpdateTelegramBotCommandDto dto)
        {

            var command = await _context.BotCommands.FindAsync(dto.Id);
            if (command == null)
            {
                return NotFound("Не знайшли такої команди, за твоїм 'id'");
            }

            // Обновляем только переданные поля
            if (dto.Command != null) command.Command = dto.Command;
            if (dto.Description != null) command.Description = dto.Description;
            if (dto.Response != null) command.Response = dto.Response;

            var botSettingsExists = await _context.BotSettings.AnyAsync(b => b.Id == dto.BotSettingsId);
            if (!botSettingsExists && dto.BotSettingsId != 0) // если 0 — разрешаем (опционально)
            {
                return BadRequest("BotSettings with the specified ID does not exist.");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommandExists(dto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/telegrambotcommands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommand(int id)
        {
            var command = await _context.BotCommands.FindAsync(id);
            if (command == null)
            {
                return NotFound();
            }

            _context.BotCommands.Remove(command);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommandExists(int id)
        {
            return _context.BotCommands.Any(e => e.Id == id);
        }

    }
}
