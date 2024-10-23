using Data;
using Data.Tables;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly Connection _connection;
        private readonly JwtService _jwtService;
        private readonly RabbitMqService _rabbitMqService;

        public RegisterUserCommandHandler(Connection connection, JwtService jwtService, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _jwtService = jwtService;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var userDto = request.UserRegistrationDto; // Теперь это будет работать

            // Проверка, существует ли уже пользователь с таким email
            var existingUser = await _connection.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Пользователь с таким email уже зарегистрирован.");
            }

            // Хэширование пароля
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            // Поиск роли для пользователя
            var role = await _connection.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
            if (role == null)
            {
                throw new InvalidOperationException("Роль пользователя не найдена.");
            }

            // Создание нового пользователя
            var newUser = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Password = hashedPassword,
                RoleID = role.RoleID,
                CreatedAt = DateTime.UtcNow
            };

            // Сохранение пользователя в базу данных
            _connection.Users.Add(newUser);
            await _connection.SaveChangesAsync(cancellationToken);

            // Генерация JWT токена для нового пользователя
            var token = _jwtService.GenerateJWT(newUser);

            // Отправка сообщения в RabbitMQ
            var message = $"Новый пользователь зарегистрирован: {newUser.Email}";
            _rabbitMqService.SendMessage(message); // Отправка сообщения

            // Возвращаем токен
            return token;
        }
    }
}
