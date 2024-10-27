using Data;
using Data.Tables;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Commands;
using SuperServerRIT.Model;
using System.Threading;
using System.Threading.Tasks;

namespace SuperServerRIT.Handlers
{
    public class GetNotificationByIdHandler : IRequestHandler<GetNotificationByIdCommand, NotificationDto>
    {
        private readonly Connection _connection;

        public GetNotificationByIdHandler(Connection connection)
        {
            _connection = connection;
        }

        public async Task<NotificationDto> Handle(GetNotificationByIdCommand request, CancellationToken cancellationToken)
        {
            var notification = await _connection.Notification
                .Include(n => n.Equipment)
                .FirstOrDefaultAsync(n => n.NotificationID == request.NotificationID, cancellationToken);

            if (notification == null)
            {
                return null; 
            }

            return new NotificationDto
            {
                EquipmentId = notification.EquipmentID,
                Message = notification.Message,
                Timestamp = notification.Timestamp
            };
        }
    }
}
