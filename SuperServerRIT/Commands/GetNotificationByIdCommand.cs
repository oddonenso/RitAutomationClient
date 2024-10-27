using MediatR;
using SuperServerRIT.Model;

namespace SuperServerRIT.Commands
{
    public class GetNotificationByIdCommand : IRequest<NotificationDto>
    {
        public int NotificationID { get; set; }

        public GetNotificationByIdCommand(int notificationId)
        {
            NotificationID = notificationId;
        }
    }
}
