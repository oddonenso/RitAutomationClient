using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using BCrypt.Net;
using Data;
using SuperServerRIT.Model;
using Data.Tables;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using SuperServerRIT.Services;

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly Connection _connection;
        private readonly IConfiguration _config;
        private readonly JwtService _jwtService; // Внедрение JwtService

        public AuthController(Connection connection, IConfiguration config, JwtService jwtService)
        {
            _connection = connection;
            _config = config;
            _jwtService = jwtService; // Внедрение через конструктор
        }

        //регистратион
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
        {
            var user = await _connection.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user != null)
            {
                return BadRequest(new { message = "Пользователь с таким эмэйлом уже зарегистрирован в системе:(" });
            }

            string hashPass = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var role = await _connection.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
            if (role == null)
            {
                return BadRequest(new { message = "Роль пользователя не найдена в системе" });
            }

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = hashPass,
                RoleID = role.RoleID, //по умолчанию
                CreatedAt = DateTime.UtcNow
            };

            _connection.Users.Add(newUser);
            await _connection.SaveChangesAsync();

            // Генерация JWT-токена для user через JwtService
            var token = _jwtService.GenerateJWT(newUser);

            return Ok(new
            {
                message = "Регистрация пройдена",
                token = token
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
        {
            var user = await _connection.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Refresh Token недействителен" });
            }

            // Генерация нового Access Token через JwtService
            var token = _jwtService.GenerateJWT(user);
            var newRefreshToken = GenerateRefreshToken();

            // Обновление Refresh Token в базе данных
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30); // Устанавливаем новый срок действия

            await _connection.SaveChangesAsync();

            return Ok(new { token, refreshToken = newRefreshToken });
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var user = await _connection.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized(new { message = "Неверный email или пароль:(" });
            }

            // Генерация токенов через JwtService
            var token = _jwtService.GenerateJWT(user);
            var refreshToken = GenerateRefreshToken();

            // Сохранение Refresh Token в базе данных
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30); //срок действия

            await _connection.SaveChangesAsync(); // save changes

            return Ok(new
            {
                message = "Вход выполнен",
                token = token,
                refreshToken = refreshToken // return Refresh Token client
            });
        }

        [Authorize]
        [HttpGet("secure-endpoint")]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "Это защищенные данные" });
        }
    }
}
