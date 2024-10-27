using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Commands;
using SuperServerRIT.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SuperServerRIT.Handlers
{
    public class GetAllNotificationsHandler : IRequestHandler<GetAllNotificationsCommand, List<NotificationDto>>
    {
        private readonly Connection _connection;

        public GetAllNotificationsHandler(Connection connection)
        {
            _connection = connection;
        }

        public async Task<List<NotificationDto>> Handle(GetAllNotificationsCommand request, CancellationToken cancellationToken)
        {
            var notifications = await _connection.Notification.Include(x => x.Equipment).ToListAsync(cancellationToken);

            return notifications.Select(n => new NotificationDto
            {
                EquipmentId = n.EquipmentID,
                Message = n.Message,
                Timestamp = n.Timestamp
            }).ToList();
        }
    }
}
