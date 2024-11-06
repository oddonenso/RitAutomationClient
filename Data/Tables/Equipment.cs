using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tables
{
    public class Equipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EquipmentID")]
        public int EquipmentID { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        [ForeignKey("Type")]
        public int TypeId { get; set; } 
        public Type Type { get; set; } = null!;
        public Status Status { get; set; } = null!;
        public ICollection<EquipmentStatus> EquipmentStatuses { get; set; } = null!;
    }
}
