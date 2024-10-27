using MediatR;
using Data;
using Data.Tables;
using Microsoft.AspNetCore.JsonPatch;
using SuperServerRIT.Commands;

namespace SuperServerRIT.Handlers
{
    public class UpdateEquipmentHandler : IRequestHandler<UpdateEquipmentCommand, string>
    {
        private readonly Connection _connection;

        public UpdateEquipmentHandler(Connection connection)
        {
            _connection = connection;
        }

        public async Task<string> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _connection.Equipment.FindAsync(request.EquipmentId);
            if (equipment == null)
            {
                throw new Exception("Оборудование не найдено");
            }

            request.PatchDocument.ApplyTo(equipment);
            await _connection.SaveChangesAsync(cancellationToken);

            return "Оборудование обновлено";
        }
    }
}
