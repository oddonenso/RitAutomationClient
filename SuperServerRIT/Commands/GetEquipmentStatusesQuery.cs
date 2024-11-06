using Data;
using Data.Tables;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace SuperServerRIT.Commands
{
    public class GetEquipmentStatusesQuery : IRequest<List<Status>>
    {
    }

    public class GetEquipmentStatusesQueryHandler : IRequestHandler<GetEquipmentStatusesQuery, List<Status>>
    {
        private readonly Connection _context;

        public GetEquipmentStatusesQueryHandler(Connection context)
        {
            _context = context;
        }

        public async Task<List<Status>> Handle(GetEquipmentStatusesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Status.ToListAsync(cancellationToken);
        }
    }
}
