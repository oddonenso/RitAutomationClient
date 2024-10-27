// Handlers/UpdateEquipmentStatusHandler.cs
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
    public class UpdateEquipmentStatusHandler : IRequestHandler<UpdateEquipmentStatusCommand, bool>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public UpdateEquipmentStatusHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<bool> Handle(UpdateEquipmentStatusCommand request, CancellationToken cancellationToken)
        {
            var equipmentStatus = await _connection.EquipmentStatus.FindAsync(request.EquipmentStatusID);
            if (equipmentStatus == null) return false;

            if (request.Temperature.HasValue) equipmentStatus.Temperature = request.Temperature.Value;
            if (request.Pressure.HasValue) equipmentStatus.Pressure = request.Pressure.Value;
            if (!string.IsNullOrEmpty(request.Location)) equipmentStatus.Location = request.Location;
            if (!string.IsNullOrEmpty(request.Status)) equipmentStatus.Status = request.Status;

            await _connection.SaveChangesAsync();

            // Отправка сообщения в RabbitMQ
            var message = $"Обновлен статус оборудования: {equipmentStatus.EquipmentStatusID}";
            _rabbitMqService.SendMessage(message);

            return true;
        }
    }
}
