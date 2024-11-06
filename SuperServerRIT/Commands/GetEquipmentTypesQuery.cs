using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace SuperServerRIT.Commands
{
    public class GetEquipmentTypesQuery : IRequest<List<Data.Tables.Type>>  
    {
    }

    public class GetEquipmentTypesQueryHandler : IRequestHandler<GetEquipmentTypesQuery, List<Data.Tables.Type>>
    {
        private readonly Connection _context;

        public GetEquipmentTypesQueryHandler(Connection context)
        {
            _context = context;
        }

        public async Task<List<Data.Tables.Type>> Handle(GetEquipmentTypesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Type.ToListAsync(cancellationToken);  
        }
    }
}
