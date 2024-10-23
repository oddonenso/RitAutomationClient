using Data;
using Data.Tables;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpGet]
        public async Task<IActionResult> GetAllEquipmentStatus()
        {
            try
            {
                var statusList = await _connection.EquipmentStatus.Include(x => x.Equipment).ToListAsync();
                return Ok(statusList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipmentStatusById(int id)
        {
            try
            {
                var status = await _connection.EquipmentStatus.Include(x => x.Equipment).FirstOrDefaultAsync(s => s.EquipmentStatusID == id);
                if (status == null)
                {
                    return NotFound(new { message = "Статус оборудования не найден" });
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEquipmentStatus([FromBody] EquipmentStatusDto request)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEquipmentStatus(int id, [FromBody] JsonPatchDocument<EquipmentStatus> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    return BadRequest(new { message = "Неверный Patch документ" });
                }

                var status = await _connection.EquipmentStatus.FindAsync(id);
                if (status == null)
                {
                    return NotFound(new { message = "Статус оборудования не найден" });
                }

                patchDoc.ApplyTo(status);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _connection.SaveChangesAsync();
                return Ok(new { message = "Статус оборудования успешно обновлен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipmentStatus(int id, [FromBody] EquipmentStatusUpdateDto request)
        {
            try
            {
                var status = await _connection.EquipmentStatus.FindAsync(id);
                if (status == null)
                {
                    return NotFound(new { message = "Статус оборудования не найден" });
                }

                status.Temperature = request.Temperature ?? status.Temperature;
                status.Pressure = request.Pressure ?? status.Pressure;
                status.Location = request.Location ?? status.Location;
                status.Status = request.Status ?? status.Status;
                status.Timestamp = DateTime.UtcNow;

                await _connection.SaveChangesAsync();

                return Ok(new { message = "Статус оборудования обновлен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipmentStatus(int id)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }
    }
}
