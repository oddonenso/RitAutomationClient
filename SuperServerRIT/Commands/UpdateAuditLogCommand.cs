using MediatR;

namespace SuperServerRIT.Commands
{
    public class UpdateAuditLogCommand : IRequest<bool>
    {
        public int LogID { get; set; }
        public int UserID { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityAffected { get; set; } = string.Empty;
        public int EntityID { get; set; }
    }
}
