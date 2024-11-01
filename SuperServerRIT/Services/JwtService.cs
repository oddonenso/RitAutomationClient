using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Data.Tables;

namespace SuperServerRIT.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration не может быть null");
        }

        public string GetTokenFromStorage()
        {
            var token = _config["Jwt:StoredToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Токен отсутствует или пуст.");
            }
            return token;
        }

        public string GenerateJWT(User user)
        {
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer))
                throw new Exception("JWT ключ или Issuer не могут быть пустыми! Проверьте конфигурацию.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Упрощённые утверждения токена
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName ?? "Unknown Name"),
                new Claim(ClaimTypes.Email, user.Email ?? "Unknown Email")
            };

            var tokenLifetime = _config.GetValue<int>("Jwt:Lifetime", 3);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.Now.AddHours(tokenLifetime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string jwtToken)
        {
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];

            if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer))
                throw new Exception("Необходимые параметры для проверки токена отсутствуют.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
            };

            try
            {
                tokenHandler.ValidateToken(jwtToken, validationParameters, out _);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке токена: {ex.Message}");
                return false;
            }
        }
    }
}
