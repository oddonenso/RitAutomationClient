using MediatR;

namespace SuperServerRIT.Commands
{
    public class DeleteAuditLogCommand : IRequest<bool>
    {
        public int LogID { get; set; }
    }
}
