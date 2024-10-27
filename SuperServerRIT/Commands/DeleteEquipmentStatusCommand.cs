using MediatR;

namespace SuperServerRIT.Commands
{
    public class DeleteEquipmentStatusCommand : IRequest<bool>
    {
        public int EquipmentStatusID { get; set; }
    }
}
