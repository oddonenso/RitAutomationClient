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
            var newEquipment = new Equipment
            {
                Name = request.Name,
                Type = request.Type,
                Status = request.Status // Используем статус из запроса
            };

            _connection.Equipment.Add(newEquipment);
            await _connection.SaveChangesAsync(cancellationToken);

            // Отправляем сообщение в RabbitMQ
            var message = new { Action = "EquipmentCreated", EquipmentId = newEquipment.EquipmentID, newEquipment.Name, newEquipment.Type };
            _rabbitMqService.SendMessage(message);

            return "Equipment created successfully";
        }
    }
}