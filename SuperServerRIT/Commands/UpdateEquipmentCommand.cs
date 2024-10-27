using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Data.Tables;

namespace SuperServerRIT.Commands
{
    public class UpdateEquipmentCommand : IRequest<string>
    {
        public int EquipmentId { get; set; }
        public JsonPatchDocument<Equipment> PatchDocument { get; set; } = null!;
    }
}
