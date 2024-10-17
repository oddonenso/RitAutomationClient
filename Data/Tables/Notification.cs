using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("NotificationID")]
        public int NotificationID { get; set; }

        [ForeignKey("Equipment")]
        public int EquipmentID { get; set; }

        public Equipment Equipment { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Message { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } 
    }
}
