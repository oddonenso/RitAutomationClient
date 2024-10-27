using Data.Tables;
using MediatR;

namespace SuperServerRIT.Commands
{
    public class GetAllEquipmentCommand : IRequest<List<Equipment>>
    {
    }
}
