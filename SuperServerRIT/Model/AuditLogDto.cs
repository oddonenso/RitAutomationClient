namespace SuperServerRIT.Model
{
    public class AuditLogDto
    {
        public int UserID { get; set; }  

        public string Action { get; set; } = string.Empty;  

        public string EntityAffected { get; set; } = string.Empty;  

        public int EntityID { get; set; }  

        public DateTime Timestamp { get; set; }  
    }

}
