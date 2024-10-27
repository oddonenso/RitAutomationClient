using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class DeleteNotificationHandler : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public DeleteNotificationHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await _connection.Notification.FindAsync(request.NotificationID);
            if (notification == null) return false;

            _connection.Notification.Remove(notification);
            await _connection.SaveChangesAsync(cancellationToken);

            var message = $"Удалено уведомление: {notification.NotificationID}";
            _rabbitMqService.SendMessage(message);

            return true;
        }
    }
}
