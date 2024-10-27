// Handlers/AddAuditLogHandler.cs
using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class AddAuditLogHandler : IRequestHandler<AddAuditLogCommand, int>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public AddAuditLogHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<int> Handle(AddAuditLogCommand request, CancellationToken cancellationToken)
        {
            var log = new AuditLog
            {
                UserID = request.UserID,
                Action = request.Action,
                EntityAffected = request.EntityAffected,
                EntityID = request.EntityID,
                Timestamp = DateTime.UtcNow
            };

            _connection.AuditLog.Add(log);
            await _connection.SaveChangesAsync();

            // Отправка сообщения в RabbitMQ
            var message = $"Добавлена запись аудита: {log.LogID}";
            _rabbitMqService.SendMessage(message);

            return log.LogID;
        }
    }
}
