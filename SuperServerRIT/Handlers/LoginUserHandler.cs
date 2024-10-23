using MediatR;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Data;
using Data.Tables;
using SuperServerRIT.Services;
using SuperServerRIT.Commands;
using System.Security.Cryptography;

namespace SuperServerRIT.Handlers
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly Connection _connection;
        private readonly JwtService _jwtService;
        private readonly RabbitMqService _rabbitMqService;

        public LoginUserHandler(Connection connection, JwtService jwtService, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _jwtService = jwtService;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _connection.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new Exception("Неверный email или пароль");
            }

            var token = _jwtService.GenerateJWT(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
            await _connection.SaveChangesAsync();

            // пользователь Zашел
            var message = $"Пользователь {user.Email} вошел в систему";
            _rabbitMqService.SendMessage(message);

            return new LoginUserResponse
            {
                Message = "Вход выполнен",
                Token = token,
                RefreshToken = refreshToken
            };
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
    }
}
