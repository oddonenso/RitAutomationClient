using MediatR;
using SuperServerRIT.Model;

namespace SuperServerRIT.Commands
{
    public class GetAllNotificationsCommand : IRequest<List<NotificationDto>>
    {
    }
}
