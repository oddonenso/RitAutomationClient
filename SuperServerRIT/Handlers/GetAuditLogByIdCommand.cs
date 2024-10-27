
using Data;
using Data.Tables;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Commands;
using SuperServerRIT.Model;
using System.Threading;
using System.Threading.Tasks;

namespace SuperServerRIT.Handlers
{
    public class GetAuditLogByIdHandler : IRequestHandler<GetAuditLogByIdCommand, AuditLogDto>
    {
        private readonly Connection _connection;

        public GetAuditLogByIdHandler(Connection connection)
        {
            _connection = connection;
        }

        public async Task<AuditLogDto> Handle(GetAuditLogByIdCommand request, CancellationToken cancellationToken)
        {
            var log = await _connection.AuditLog
                .Include(l => l.User) 
                .FirstOrDefaultAsync(l => l.LogID == request.LogID, cancellationToken);

            if (log == null)
            {
                return null; 
            }

            return new AuditLogDto
            {
                UserID = log.UserID,
                Action = log.Action,
                EntityAffected = log.EntityAffected,
                EntityID = log.EntityID,
                Timestamp = log.Timestamp
            };
        }
    }
}
