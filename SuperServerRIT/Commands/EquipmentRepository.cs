using Data;
using Data.Tables;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperServerRIT.Commands
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly Connection _context;

        public EquipmentRepository(Connection context)
        {
            _context = context;
        }

        public async Task<Equipment> GetByIdAsync(int id)
        {
            return await _context.Equipment.FindAsync(id);
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            _context.Entry(equipment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            return await _context.Equipment.ToListAsync();
        }
    }
}
