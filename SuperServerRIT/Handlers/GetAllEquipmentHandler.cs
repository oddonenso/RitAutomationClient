using MediatR;
using Data;
using Data.Tables;
using SuperServerRIT.Commands;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SuperServerRIT.Handlers
{
    public class GetAllEquipmentHandler : IRequestHandler<GetAllEquipmentCommand, List<Equipment>>
    {
        private readonly Connection _connection;

        public GetAllEquipmentHandler(Connection connection)
        {
            _connection = connection;
        }

        public async Task<List<Equipment>> Handle(GetAllEquipmentCommand request, CancellationToken cancellationToken)
        {
            return await _connection.Equipment.ToListAsync(cancellationToken);
        }
    }
}
