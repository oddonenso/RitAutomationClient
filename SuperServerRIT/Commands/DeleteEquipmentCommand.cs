using MediatR;

namespace SuperServerRIT.Commands
{
    public class DeleteEquipmentCommand : IRequest<string>
    {
        public int EquipmentId { get; set; }
    }
}
