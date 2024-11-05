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

            if (request.PatchDocument != null)
            {
                
                request.PatchDocument.ApplyTo(equipment);
                var debugInfo = $"Updated equipment details:\nName: {equipment.Name}\nType: {equipment.Type}\nStatus: {equipment.Status}";
                Console.WriteLine(debugInfo); 
            }
            else
            {
                throw new ArgumentException("Patch document cannot be null or empty.");
            }

            await _repository.UpdateAsync(equipment);
            return $"Equipment with ID {request.EquipmentId} has been updated.";
        }

    }
}