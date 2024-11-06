using MediatR;

namespace SuperServerRIT.Commands
{
    public class AddEquipmentStatusCommand : IRequest<int>
    {
        public int EquipmentID { get; set; }
        public decimal Temperature { get; set; }
        public decimal Pressure { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
