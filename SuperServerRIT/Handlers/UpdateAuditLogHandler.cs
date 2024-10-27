using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class UpdateNotificationHandler : IRequestHandler<UpdateNotificationCommand, bool>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public UpdateNotificationHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<bool> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await _connection.Notification.FindAsync(request.NotificationID);
            if (notification == null) return false;

           
            if (request.EquipmentId.HasValue)
            {
                notification.EquipmentID = request.EquipmentId.Value; 
            }

            notification.Message = request.Message; 
            notification.Timestamp = DateTime.UtcNow;

            await _connection.SaveChangesAsync(cancellationToken);

            var message = $"Обновлено уведомление: {notification.NotificationID}";
            _rabbitMqService.SendMessage(message);

            return true;
        }

    }
}
