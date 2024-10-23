using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Data.Tables;

namespace SuperServerRIT.Commands
{
    public class UpdateUserCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public JsonPatchDocument<User> PatchDocument { get; set; } = null!;
    }
}
