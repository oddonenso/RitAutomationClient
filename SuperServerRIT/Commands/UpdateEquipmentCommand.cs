using Data.Tables;
using MediatR;
using Microsoft.AspNetCore.JsonPatch; 

public class UpdateEquipmentCommand : IRequest<string>
{
    public int EquipmentId { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public JsonPatchDocument<Equipment> PatchDocument { get; set; } = null!;
}
