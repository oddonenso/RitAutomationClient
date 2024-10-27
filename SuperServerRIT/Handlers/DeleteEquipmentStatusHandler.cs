using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Data;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class DeleteEquipmentStatusHandler : IRequestHandler<DeleteEquipmentStatusCommand, bool>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public DeleteEquipmentStatusHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<bool> Handle(DeleteEquipmentStatusCommand request, CancellationToken cancellationToken)
        {
            var equipmentStatus = await _connection.EquipmentStatus.FindAsync(request.EquipmentStatusID);
            if (equipmentStatus == null) return false;

            _connection.EquipmentStatus.Remove(equipmentStatus);
            await _connection.SaveChangesAsync();

            
            var message = $"Удален статус оборудования: {equipmentStatus.EquipmentStatusID}";
            _rabbitMqService.SendMessage(message);

            return true;
        }
    }
}
