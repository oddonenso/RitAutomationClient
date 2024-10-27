using MediatR;

namespace SuperServerRIT.Commands
{
    public class AddNotificationCommand : IRequest<int>
    {
        public int EquipmentId { get; set; }
        public string Message { get; set; }

        public AddNotificationCommand(int equipmentId, string message)
        {
            EquipmentId = equipmentId;
            Message = message;
        }
    }
}
