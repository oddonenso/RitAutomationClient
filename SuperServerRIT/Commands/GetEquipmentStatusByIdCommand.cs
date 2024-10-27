using MediatR;
using SuperServerRIT.Model;

namespace SuperServerRIT.Commands
{
    public class GetEquipmentStatusByIdCommand : IRequest<EquipmentStatusDto>
    {
        public int EquipmentStatusID { get; set; }
    }
}
