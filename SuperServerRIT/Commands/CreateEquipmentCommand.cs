using MediatR;

namespace SuperServerRIT.Commands
{
    public class CreateEquipmentCommand : IRequest<string>
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "New"; // Начальный статус по умолчанию
    }
}
