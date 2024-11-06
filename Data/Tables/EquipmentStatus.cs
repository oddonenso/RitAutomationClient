using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    public class EquipmentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EquipmentStatusID")]
        public int EquipmentStatusID { get; set; }

        [ForeignKey("Equipment")]
        public int EquipmentID { get; set; }

        public Equipment Equipment { get; set; } = null!;

        public decimal Temperature { get; set; }

        public decimal Pressure { get; set; }

        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;


        public DateTime Timestamp { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }
    }
}
