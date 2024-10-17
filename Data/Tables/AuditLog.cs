using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("LogID")]
        public int LogID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; } 

        public User User { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Action { get; set; } = null!;

        [MaxLength(50)]
        public string EntityAffected { get; set; } = null!;

        public int EntityID { get; set; } 

        public DateTime Timestamp { get; set; } 
    }
}
