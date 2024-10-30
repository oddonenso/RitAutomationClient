using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using Microsoft.AspNetCore.JsonPatch;
using SuperServerRIT.Services;

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
                throw new Exception("Пользователь не найден");

            request.PatchDocument.ApplyTo(user);
            await _connection.SaveChangesAsync();

            _rabbitMqService.SendMessage($"Пользователь {user.Email} обновлен");
            return "Пользователь обновлен";
        }
    }
}
