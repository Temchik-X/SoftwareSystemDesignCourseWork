using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using BlazorApp2.Shared;
using Microsoft.EntityFrameworkCore;
using BlazorApp2.Server.Data;
using System.Security.Cryptography;
using System.Data;

namespace BlazorApp2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // Поиск пользователя по имени пользователя в базе данных
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            // Проверка, найден ли пользователь
            if (user != null)
            {
                // Сравнение хеша пароля из базы данных с введенным паролем
                if (VerifyPassword(model.Password, user.PasswordHash))
                {
                    var token = GenerateJwtToken(model.Username, user.Role, user.UserId);
                    return Ok(new { Token = token });
                }
            }

            return Unauthorized();
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                // Проверяем, существует ли уже пользователь с таким же именем
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    return BadRequest("Пользователь с таким именем уже существует.");
                }

                // Создаем нового пользователя
                var newUser = new User
                {
                    Username = model.Username,
                    Role = model.Role,
                    PasswordHash = HashPassword(model.Password)
                    // Здесь можно добавить другие поля пользователя, если необходимо
                };

                // Сохраняем пользователя в базе данных
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok("Пользователь успешно зарегистрирован.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя.");
                return StatusCode(500, "Ошибка при регистрации пользователя.");
            }
        }
        // Метод для хеширования пароля
        private string HashPassword(string password)
        {
            // Пример хеширования пароля с использованием SHA256
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
        // Метод для проверки соответствия паролей
        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Пример проверки паролей с использованием SHA256 хеширования
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
            var enteredPasswordHash = Convert.ToBase64String(hashedBytes);
            return enteredPasswordHash == storedPasswordHash;
        }

        private string GenerateJwtToken(string username, string role, int id)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.SerialNumber, id.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role) // Добавление роли как утверждения
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // Метод API, который получает токен и возвращает имя пользователя
        [HttpGet("get-username")]
        [Authorize]
        public IActionResult GetUsernameFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Здесь произведите разбор токена и извлеките имя пользователя
            string username = ParseTokenForUsername(token);

            // Возврат имени пользователя в ответе API
            return Ok(new { Username = username });
        }

        // Метод API, который получает токен и возвращает роль пользователя
        [HttpGet("get-role")]
        [Authorize]
        public IActionResult GetRoleFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Здесь произведите разбор токена и извлеките роль пользователя
            string role = ParseTokenForRole(token);

            // Возврат роли пользователя в ответе API
            return Ok(new { Role = role });
        }
        [HttpGet("get-id")]
        [Authorize]
        public IActionResult GetIdFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Здесь произведите разбор токена и извлеките роль пользователя
            string id = ParseTokenForId(token);

            // Возврат роли пользователя в ответе API
            return Ok(new { Id = id });
        }

        // Метод для разбора токена и извлечения имени пользователя
        private string ParseTokenForUsername(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Получаем имя пользователя из токена
            var username = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            return username;
        }

        // Метод для разбора токена и извлечения роли пользователя
        private string ParseTokenForRole(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            // Получаем роль пользователя из токена
            var role = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return role;
        }
        private string ParseTokenForId(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            // Получаем роль пользователя из токена
            var id = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
            return id;
        }
    }
}
