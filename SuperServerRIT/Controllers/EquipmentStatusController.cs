using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Model;

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentStatusController : Controller
    {
        private readonly Connection _connection;
        public EquipmentStatusController(Connection connection)
        {
            _connection = connection;
        }

        //get all status

        [HttpGet]
        public async Task<IActionResult> GetAllEquipmentStatus()
        {
            var statusList = _connection.EquipmentStatus.Include(x => x.Equipment).ToListAsync();
            return Ok(statusList);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetEquipmentStatusById(int id)
        {
            var status = await _connection.EquipmentStatus.Include(x => x.Equipment).FirstOrDefaultAsync(s => s.EquipmentStatusID == id);
            return Ok(status);
        }
        

        //create new status for equipment
        [HttpPost]

        public async Task<IActionResult> CreateEquipmentStatus([FromBody] EquipmentStatusDto request)
        {
            var equipment = await _connection.Equipment.FindAsync(request.EquipmentID);
            if (equipment == null)
            {
                return BadRequest(new { message = "Оборудование не найдено" });
            }

            var newStatus = new EquipmentStatus
            {
                EquipmentID = request.EquipmentID,
                Temperature = request.Temperature,
                Pressure = request.Pressure,
                Location = request.Location,
                Status = request.Status,
                Timestamp = DateTime.UtcNow
            };
            _connection.EquipmentStatus.Add(newStatus);
            await _connection.SaveChangesAsync();

            return Ok(new { message = "Статус оборудования создан", id = newStatus.EquipmentStatusID });
        }

        //update 
        [HttpPut("{id}")]
        public async Task<IActionResult>UpdateEquipmentStatus(int id, [FromBody] EquipmentStatusUpdateDto request)
        {
            var status = await _connection.EquipmentStatus.FindAsync(id);

            if(status == null)
            {
                return NotFound(new { message = "Статус оборудования не найден(" });
            }

            //update data
            status.Temperature = request.Temperature?? status.Temperature;
            status.Pressure = request.Pressure ?? status.Pressure;
            status.Location = request.Location ?? status.Location;  
            status.Status = request.Status ?? status.Status;
            status.Timestamp = DateTime.UtcNow;

            await _connection.SaveChangesAsync();

            return Ok(new { message = "Статус оборудования обновлен" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipmentStatus(int id)
        {
            var status = await _connection.EquipmentStatus.FindAsync(id);
            if (status == null)
            {
                return NotFound(new { message = "Статус оборудования не найден" });
            }

            _connection.EquipmentStatus.Remove(status);
            await _connection.SaveChangesAsync();

            return Ok(new { message = "Статус оборудования удален" });
        }
    }
}
