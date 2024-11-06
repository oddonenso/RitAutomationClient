using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                Console.WriteLine($"Пытаемся авторизовать пользователя: {request.Email}");

                var user = await _connection.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

                if (user == null)
                {
                    Console.WriteLine("Пользователь с таким email не найден.");
                    throw new Exception("Неверный email или пароль.");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    Console.WriteLine("Неверный пароль.");
                    throw new Exception("Неверный email или пароль.");
                }

                var token = _jwtService.GenerateJWT(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
                await _connection.SaveChangesAsync();

                Console.WriteLine("Отправка сообщения в RabbitMQ о входе пользователя...");
                _rabbitMqService.SendMessage($"Пользователь {user.Email} вошел в систему");

                return new LoginUserResponse { Message = "Вход выполнен", Token = token, RefreshToken = refreshToken };
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Ошибка базы данных при обновлении пользователя: {ex.Message}");
                throw new Exception("Ошибка базы данных. Обновление пользователя не удалось.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при авторизации пользователя: {ex.Message}");
                throw new Exception($"Ошибка при авторизации пользователя: {ex.Message}");
            }
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
