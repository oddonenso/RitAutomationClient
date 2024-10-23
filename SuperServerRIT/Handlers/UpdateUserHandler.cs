using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Data;
using Data.Tables;
using SuperServerRIT.Services;
using SuperServerRIT.Commands;

namespace SuperServerRIT.Handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, string>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public UpdateUserHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<string> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _connection.Users.FindAsync(request.UserId);
            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }

            request.PatchDocument.ApplyTo(user);
            await _connection.SaveChangesAsync();

            // отправка мессаге, что юзер обновлен
            var message = $"Пользователь {user.Email} обновлен";
            _rabbitMqService.SendMessage(message);

            return "Пользователь обновлен";
        }
    }
}
