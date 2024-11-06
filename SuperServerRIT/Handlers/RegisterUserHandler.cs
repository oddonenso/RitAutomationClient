using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace SuperServerRIT.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly Connection _connection;
        private readonly JwtService _jwtService;
        private readonly RabbitMqService _rabbitMqService;

        public RegisterUserHandler(Connection connection, JwtService jwtService, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _jwtService = jwtService;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var userDto = request.UserRegistrationDto;
            var existingUser = await _connection.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (existingUser != null)
                throw new InvalidOperationException("Пользователь с таким email уже зарегистрирован.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var role = await _connection.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");

            var newUser = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                RoleID = role.RoleID,
                CreatedAt = DateTime.UtcNow
            };

            _connection.Users.Add(newUser);
            await _connection.SaveChangesAsync(cancellationToken);

            var token = _jwtService.GenerateJWT(newUser);
            _rabbitMqService.SendMessage($"Новый пользователь зарегистрирован: {newUser.Email}");

            return token;
        }
    }
}
