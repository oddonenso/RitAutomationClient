using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Model;

namespace SuperServerRIT.Commands
{
    public class GetAllEquipmentStatusesForMapCommand : IRequest<List<EquipmentStatusDto>>
    {
    }

    public class GetAllEquipmentStatusesForMapHandler : IRequestHandler<GetAllEquipmentStatusesForMapCommand, List<EquipmentStatusDto>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GetAllEquipmentStatusesForMapHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<EquipmentStatusDto>> Handle(GetAllEquipmentStatusesForMapCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Connection>();
                var equipmentStatuses = await dbContext.EquipmentStatus.ToListAsync();

                var equipmentStatusDtos = new List<EquipmentStatusDto>();

                foreach (var status in equipmentStatuses)
                {
                    equipmentStatusDtos.Add(new EquipmentStatusDto
                    {
                        EquipmentStatusID = status.EquipmentStatusID,
                        EquipmentID = status.EquipmentID,
                        Location = status.Location,
                        Status = status.Status,
                        Temperature = status.Temperature,
                        Pressure = status.Pressure,
                        Timestamp = status.Timestamp,
                        Latitude = status.Latitude,   
                        Longitude = status.Longitude  
                    });
                }

                return equipmentStatusDtos;
            }
        }
    }
}
