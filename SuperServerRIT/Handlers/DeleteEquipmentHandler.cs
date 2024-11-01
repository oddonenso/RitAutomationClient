using MediatR;
using Data;
using SuperServerRIT.Commands;
using SuperServerRIT.Services;

namespace SuperServerRIT.Handlers
{
    public class DeleteEquipmentHandler : IRequestHandler<DeleteEquipmentCommand, string>
    {
        private readonly Connection _connection;
        private readonly RabbitMqService _rabbitMqService;

        public DeleteEquipmentHandler(Connection connection, RabbitMqService rabbitMqService)
        {
            _connection = connection;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<string> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _connection.Equipment.FindAsync(request.EquipmentId);
            if (equipment == null)
            {
                throw new Exception("Equipment not found");
            }

            _connection.Equipment.Remove(equipment);
            await _connection.SaveChangesAsync(cancellationToken);

            // Отправляем сообщение в RabbitMQ
            var message = new { Action = "EquipmentDeleted", EquipmentId = request.EquipmentId };
            _rabbitMqService.SendMessage(message);

            return "Equipment deleted successfully";
        }
    }
}
