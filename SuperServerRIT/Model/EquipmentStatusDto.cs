namespace SuperServerRIT.Model
{
    public class EquipmentStatusDto
    {
        public int EquipmentID { get; set; }
        public decimal Temperature { get; set; }
        public decimal Pressure { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "Offline"; //по дефолту
    }
}
