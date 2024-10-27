using MediatR;

namespace SuperServerRIT.Commands
{
    public class CreateEquipmentCommand : IRequest<string>
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
