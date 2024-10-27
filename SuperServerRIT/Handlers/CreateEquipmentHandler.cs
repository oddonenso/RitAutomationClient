using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;

namespace SuperServerRIT.Handlers
{
    public class CreateEquipmentHandler : IRequestHandler<CreateEquipmentCommand, string>
    {
        private readonly Connection _connection;

        public CreateEquipmentHandler(Connection connection)
        {
            _connection = connection;
        }

        public async Task<string> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var newEquipment = new Equipment
            {
                Name = request.Name,
                Type = request.Type,
                
            };

            _connection.Equipment.Add(newEquipment);
            await _connection.SaveChangesAsync(cancellationToken);

            return "Оборудование создано";
        }
    }
}
