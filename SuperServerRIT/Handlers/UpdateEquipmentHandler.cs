using MediatR;
using Data;
using Data.Tables;
using Microsoft.AspNetCore.JsonPatch;
using SuperServerRIT.Commands;

namespace SuperServerRIT.Handlers
{
    public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, string>
    {
        private readonly IEquipmentRepository _repository;

        public UpdateEquipmentCommandHandler(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _repository.GetByIdAsync(request.EquipmentId);
            if (equipment == null)
            {
                throw new NotFoundException($"Equipment with ID {request.EquipmentId} not found.");
            }

            // Применяем изменения через JsonPatchDocument
            if (request.PatchDocument != null)
            {
                request.PatchDocument.ApplyTo(equipment);
            }

            // Если поля Name, Status или Type не null, обновляем их
            if (!string.IsNullOrEmpty(request.Name))
            {
                equipment.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                equipment.Status = request.Status;
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                equipment.Type = request.Type;
            }

            await _repository.UpdateAsync(equipment);
            return $"Equipment with ID {request.EquipmentId} has been updated.";
        }
    }

}
