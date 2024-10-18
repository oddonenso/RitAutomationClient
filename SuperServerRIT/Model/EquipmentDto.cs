namespace SuperServerRIT.Model
{
    public class EquipmentDto
    {
        public int EquipmentID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;  
        public string Type {  get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
