
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class AddEquipmentStatusHandler : IRequestHandler<AddEquipmentStatusCommand, int>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public AddEquipmentStatusHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<int> Handle(AddEquipmentStatusCommand request, CancellationToken cancellationToken)
        {
            var equipmentStatus = new EquipmentStatus
            {
                EquipmentID = request.EquipmentID,
                Temperature = request.Temperature,
                Pressure = request.Pressure,
                Location = request.Location,
                Status = request.Status,
                Timestamp = request.Timestamp
            };

            _connection.EquipmentStatus.Add(equipmentStatus);
            await _connection.SaveChangesAsync();

            // Отправка сообщения в RabbitMQ
            var message = $"Добавлен статус оборудования: {equipmentStatus.EquipmentStatusID}";
            _rabbitMqService.SendMessage(message);

            return equipmentStatus.EquipmentStatusID;
        }
    }
}
