using MediatR;
using SuperServerRIT.Model;

namespace SuperServerRIT.Commands
{
    public class GetAuditLogByIdCommand : IRequest<AuditLogDto>
    {
        public int LogID { get; set; }
    }
}
