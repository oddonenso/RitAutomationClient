using MediatR;
using Data;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class DeleteAuditLogHandler : IRequestHandler<DeleteAuditLogCommand, bool>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public DeleteAuditLogHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<bool> Handle(DeleteAuditLogCommand request, CancellationToken cancellationToken)
        {
            var log = await _connection.AuditLog.FindAsync(request.LogID);
            if (log == null) return false;

            _connection.AuditLog.Remove(log);
            await _connection.SaveChangesAsync();

            var message = $"Удалена запись аудита: {log.LogID}";
            _rabbitMqService.SendMessage(message);

            return true;
        }
    }
}
