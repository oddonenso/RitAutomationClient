namespace SuperServerRIT.Model
{
    public class EquipmentStatusDto
    {
        public int EquipmentStatusID { get; set; }
        public int EquipmentID { get; set; }
        public decimal Temperature { get; set; }
        public decimal Pressure { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
