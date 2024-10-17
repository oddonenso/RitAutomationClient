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

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly Connection _connection;
        private readonly IConfiguration _config;
        

        public AuthController(Connection connection, IConfiguration config)
        {
            _connection = connection;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
        {
            var user = await _connection.Users.FirstOrDefaultAsync(u=>u.Email == request.Email);

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

            //генерация JWT-токена для user
            var token = GenerateJWT(newUser);

            return Ok(new
            {
                message = "Регистрация пройдена",
                token = token
            });


        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var user = await _connection.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized(new { message = "Неверный email or password:(" });
            }

            var token = GenerateJWT(user);
            return Ok(new
            {
                message = "Вход выполнен",
                token = token
            });
        }

        [Authorize]
        [HttpGet("secure-endpoint")]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "Это защищенные данные" });
        }



        private string GenerateJWT(User user)
        {
            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("Jwt:Key", "JWT ключ не может быть null или пустым!!!");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Динамическое создание списка claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim(ClaimTypes.Name, user.FirstName.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

            // Добавление роли только если она определена
            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.RoleID.ToString()));
            }

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3), // Время жизни токена
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
