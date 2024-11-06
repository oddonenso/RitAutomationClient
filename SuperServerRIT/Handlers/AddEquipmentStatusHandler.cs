using Data;
using Data.Tables;
using MediatR;
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
            Console.WriteLine($"Обрабатываем статус для оборудования с ID: {request.EquipmentID}");

            var equipmentStatus = new EquipmentStatus
            {
                EquipmentID = request.EquipmentID,
                Temperature = request.Temperature,
                Pressure = request.Pressure,
                Location = request.Location,
                Timestamp = request.Timestamp,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            _connection.EquipmentStatus.Add(equipmentStatus);
            await _connection.SaveChangesAsync();

            _rabbitMqService.SendMessage(equipmentStatus);
            return equipmentStatus.EquipmentStatusID;
        }

    }
}
