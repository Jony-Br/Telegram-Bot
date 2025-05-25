using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLayer.Entities;
using WebAPI.Dtos.BotUsers;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TelegramUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUsers()
        {
            var allUsersInfo = await _context.Users
                .Select(selector => new TelegramBotUserControllerDto
                {
                    Id = selector.Id,
                    TelegramUserId = selector.TelegramUserId,
                    Username = selector.Username,
                    FirstName = selector.FirstName,
                    LastName = selector.LastName,
                    RegisteredAt = selector.RegisteredAt
                })
                .ToListAsync();

            return Ok(allUsersInfo);
        }

        // GET: api/users/5
        [HttpGet("{id:int}")] // Added route constraint to resolve conflict
        public async Task<ActionResult<UserInfo>> GetUser(int id)
        {
            var currentUserExist = await _context.Users
                .AnyAsync(u => u.Id == id);
            if (currentUserExist)
            {
                var userInfo = await _context.Users
                    .Select(selector => new TelegramBotUserControllerDto
                    {
                        Id = selector.Id,
                        TelegramUserId = selector.TelegramUserId,
                        Username = selector.Username,
                        FirstName = selector.FirstName,
                        LastName = selector.LastName,
                        RegisteredAt = selector.RegisteredAt
                    })
                    .Where(u => u.Id == id)
                    .FirstOrDefaultAsync();

                return Ok(userInfo);
            }
            else
            {
                return NotFound("Пользователь с таким 'id' не найден");
            }
        }

        // GET: api/users/by-telegram-id/{telegramUserId}
        [HttpGet("by-telegram-id/{telegramUserId}")] // Changed route to avoid conflict
        public async Task<ActionResult<UserInfo>> GetByTelegramId(string telegramUserId)
        {
            var currentUserExist = await _context.Users
                .AnyAsync(u => u.TelegramUserId == telegramUserId);
            if (currentUserExist)
            {
                var userInfo = await _context.Users
                    .Select(selector => new TelegramBotUserControllerDto
                    {
                        Id = selector.Id,
                        TelegramUserId = selector.TelegramUserId,
                        Username = selector.Username,
                        FirstName = selector.FirstName,
                        LastName = selector.LastName,
                        RegisteredAt = selector.RegisteredAt
                    })
                    .Where(u => u.TelegramUserId == telegramUserId)
                    .FirstOrDefaultAsync();

                return Ok(userInfo);
            }
            else
            {
                return NotFound("Пользователь с таким 'telegramUserId' не найден");
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserInfo>> PostUser(
            [FromBody] CreateTelegramBotUserControllerDto dto)
        {
            var currentUserExist = await _context.Users
                .AnyAsync(u => u.TelegramUserId == dto.TelegramUserId);
            if (currentUserExist)
            {
                return BadRequest("Пользователь с таким 'TelegramUserId' уже существует");
            }
            else
            {
                var user = new UserInfo
                {
                    TelegramUserId = dto.TelegramUserId,
                    Username = dto.Username,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    RegisteredAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
        }

        // PUT: api/users/5
        [HttpPut]
        public async Task<ActionResult> PutUser([FromBody] UpdateTelegramBotUserControllerDto dto)
        {
            var currentUserExist = await _context.Users
                .AnyAsync(u => u.Id == dto.Id);
            if (currentUserExist)
            {
                var user = await _context.Users.FindAsync(dto.Id);
                user.TelegramUserId = dto.TelegramUserId;
                user.Username = dto.Username;
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                // Добавить в базу данных параметр, что будет указывать на время обновления информации о пользователе
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            else
            {
                return NotFound("Пользователь с таким 'id' не найден");
            }
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
