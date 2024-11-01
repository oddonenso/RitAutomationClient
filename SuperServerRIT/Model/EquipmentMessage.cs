namespace SuperServerRIT.Model
{
    public class EquipmentMessage
    {

        public string Action { get; set; } = string.Empty;
        public int EquipmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
