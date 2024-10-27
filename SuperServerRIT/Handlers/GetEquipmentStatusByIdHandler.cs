
using Data;
using Data.Tables;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SuperServerRIT.Commands;
using SuperServerRIT.Model;
using System.Threading;
using System.Threading.Tasks;

namespace SuperServerRIT.Handlers
{
    public class GetEquipmentStatusByIdHandler : IRequestHandler<GetEquipmentStatusByIdCommand, EquipmentStatusDto>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GetEquipmentStatusByIdHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<EquipmentStatusDto> Handle(GetEquipmentStatusByIdCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {

                var dbContext = scope.ServiceProvider.GetRequiredService<Connection>();
                var equipmentStatus = await dbContext.EquipmentStatus.FindAsync(request.EquipmentStatusID);

                if (equipmentStatus == null) return null;

                return new EquipmentStatusDto
                {
                    EquipmentStatusID = equipmentStatus.EquipmentStatusID,
                    EquipmentID = equipmentStatus.EquipmentID,
                    Temperature = equipmentStatus.Temperature,
                    Pressure = equipmentStatus.Pressure,
                    Location = equipmentStatus.Location,
                    Status = equipmentStatus.Status,
                    Timestamp = equipmentStatus.Timestamp
                };
            }
        }
    }
}
