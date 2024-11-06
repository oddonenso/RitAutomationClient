using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Data.Tables
{
    public class Status
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [MaxLength(50)]
        [JsonPropertyName("statusName")]
        public string statusName { get; set; } = string.Empty;


        public ICollection<Equipment> Equipments { get; set; } = null!;
    }
}
