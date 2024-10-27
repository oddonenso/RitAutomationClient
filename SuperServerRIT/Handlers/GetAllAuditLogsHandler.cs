using Data;
using Data.Tables;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Commands;
using SuperServerRIT.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SuperServerRIT.Handlers
{
    public class GetAllAuditLogsHandler : IRequestHandler<GetAllAuditLogsCommand, List<AuditLogDto>>
    {
        private readonly Connection _connection;

        public GetAllAuditLogsHandler(Connection connection)
        {
            _connection = connection;
        }

        public async Task<List<AuditLogDto>> Handle(GetAllAuditLogsCommand request, CancellationToken cancellationToken)
        {
            var logs = await _connection.AuditLog
                .Include(l => l.User) 
                .ToListAsync(cancellationToken);

            return logs.Select(log => new AuditLogDto
            {
                UserID = log.UserID,
                Action = log.Action,
                EntityAffected = log.EntityAffected,
                EntityID = log.EntityID,
                Timestamp = log.Timestamp
            }).ToList();
        }
    }
}
