using MediatR;

namespace SuperServerRIT.Commands
{
    public class UpdateEquipmentStatusCommand: IRequest<bool>
    {
        public int EquipmentStatusID { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Pressure { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
    }
}
