using MediatR;
using Data;
using SuperServerRIT.Commands;

namespace SuperServerRIT.Handlers
{
    public class DeleteEquipmentHandler : IRequestHandler<DeleteEquipmentCommand, string>
    {
        private readonly Connection _connection;

        public DeleteEquipmentHandler(Connection connection)
        {
            _connection = connection;
        }

        public async Task<string> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _connection.Equipment.FindAsync(request.EquipmentId);
            if (equipment == null)
            {
                throw new Exception("Оборудование не найдено");
            }

            _connection.Equipment.Remove(equipment);
            await _connection.SaveChangesAsync(cancellationToken);

            return "Оборудование удалено";
        }
    }
}
