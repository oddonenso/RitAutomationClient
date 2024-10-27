using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class AddNotificationHandler : IRequestHandler<AddNotificationCommand, int>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public AddNotificationHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<int> Handle(AddNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Notification
            {
                EquipmentID = request.EquipmentId,
                Message = request.Message,
                Timestamp = DateTime.UtcNow
            };

            _connection.Notification.Add(notification);
            await _connection.SaveChangesAsync(cancellationToken);

            var message = $"Добавлено уведомление: {notification.NotificationID}";
            _rabbitMqService.SendMessage(message);

            return notification.NotificationID;
        }
    }
}
