using MediatR;
using System;

namespace SuperServerRIT.Commands
{
    public class AddEquipmentStatusCommand : IRequest<int>
    {
        public int EquipmentID { get; set; }
        public decimal Temperature { get; set; }
        public decimal Pressure { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; 
    }
}
