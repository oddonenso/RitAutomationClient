using Data.Tables;

namespace SuperServerRIT.Commands
{
    public interface IEquipmentRepository
    {
        Task<Equipment> GetByIdAsync(int id);
        Task UpdateAsync(Equipment equipment);
    }
}
