using Data;
using Data.Tables;
using MediatR;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class CreateEquipmentHandler : IRequestHandler<CreateEquipmentCommand, string>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public CreateEquipmentHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<string> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipmentType = await _connection.Type.FindAsync(request.TypeId);
            if (equipmentType == null)
            {
                return $"Тип оборудования с ID {request.TypeId} не найден.";
            }

            var equipmentStatus = await _connection.Status.FindAsync(request.StatusId);
            if (equipmentStatus == null)
            {
                return $"Статус оборудования с ID {request.StatusId} не найден.";
            }

            var newEquipment = new Equipment
            {
                Name = request.Name,
                Type = equipmentType,
                Status = equipmentStatus
            };

            _connection.Equipment.Add(newEquipment);
            await _connection.SaveChangesAsync(cancellationToken);

            var message = new { Action = "EquipmentCreated", EquipmentId = newEquipment.EquipmentID, newEquipment.Name, Type = equipmentType.typeName};
            _rabbitMqService.SendMessage(message);

            return "Equipment created successfully";
        }
    }

}