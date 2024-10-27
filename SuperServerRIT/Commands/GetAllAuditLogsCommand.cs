using MediatR;
using SuperServerRIT.Model;
using System.Collections.Generic;

namespace SuperServerRIT.Commands
{
    public class GetAllAuditLogsCommand : IRequest<List<AuditLogDto>>
    {

    }
}
