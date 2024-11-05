using Data.Tables;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

public class UpdateEquipmentCommand : IRequest<string>
{
    public int EquipmentId { get; set; }
    public JsonPatchDocument<Equipment> PatchDocument { get; set; } = null!;
}

