using Data.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Connection:DbContext
    {
        public Connection(DbContextOptions<Connection> options)
        : base(options)
        {
        }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<EquipmentStatus> EquipmentStatus { get; set; }
        public DbSet<Notification> Notification { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;User Id=postgres;Password=111;Database=RitAutomation;");

        }
    }
}
