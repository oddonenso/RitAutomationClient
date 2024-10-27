using MediatR;

namespace SuperServerRIT.Commands
{
    public class DeleteNotificationCommand : IRequest<bool>
    {
        public int NotificationID { get; set; }

        public DeleteNotificationCommand(int notificationId)
        {
            NotificationID = notificationId;
        }
    }
}
