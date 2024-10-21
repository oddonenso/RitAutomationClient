namespace SuperServerRIT.Model
{
    public class EquipmentStatusUpdateDto
    {
        public decimal? Temperature { get; set; }
        public decimal? Pressure { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
    }
}
