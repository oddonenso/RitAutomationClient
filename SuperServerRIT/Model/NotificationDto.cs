namespace SuperServerRIT.Model
{
    public class NotificationDto
    {
        public int EquipmentId { get; set; }    
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
