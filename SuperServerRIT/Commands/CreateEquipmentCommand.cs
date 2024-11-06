using MediatR;

namespace SuperServerRIT.Commands
{
    public class CreateEquipmentCommand : IRequest<string>
    {
        public string Name { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public int StatusId { get; set; }
    }
}
