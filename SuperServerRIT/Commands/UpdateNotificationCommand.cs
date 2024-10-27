using MediatR;

namespace SuperServerRIT.Commands
{
    public class UpdateNotificationCommand : IRequest<bool>
    {
        public int NotificationID { get; set; }
        public int? EquipmentId { get; set; }  
        public string Message { get; set; }   

        public UpdateNotificationCommand(int notificationId, int? equipmentId, string message)
        {
            NotificationID = notificationId;
            EquipmentId = equipmentId;
            Message = message;
        }
    }
}
