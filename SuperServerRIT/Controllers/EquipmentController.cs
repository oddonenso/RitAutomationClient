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
    public class EquipmentController : Controller
    {
        private readonly Connection _connection;

        public EquipmentController(Connection connection)
        {
            _connection = connection;
        }

        [HttpGet]
        public async Task<ActionResult> GetEquipment()
        {
            try
            {
                var equipment = await _connection.Equipment.ToListAsync();
                var equipmentDTO = equipment.Select(e => new EquipmentDto
                {
                    EquipmentID = e.EquipmentID,
                    Name = e.Name,
                    Status = e.Status,
                    Type = e.Type,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                return Ok(equipmentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEquipment(int id, [FromBody] JsonPatchDocument<Equipment> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    return BadRequest(new { message = "Неверный Patch документ" });
                }

                var equipment = await _connection.Equipment.FindAsync(id);
                if (equipment == null)
                {
                    return NotFound(new { message = "Оборудование не найдено" });
                }

                patchDoc.ApplyTo(equipment);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _connection.SaveChangesAsync();
                return Ok(new { message = "Оборудование успешно обновлено" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipmentById(int id)
        {
            try
            {
                var equipment = await _connection.Equipment.FindAsync(id);
                if (equipment == null)
                {
                    return NotFound(new { message = "Оборудование не найдено" });
                }
                var equipmentDTO = new EquipmentDto
                {
                    EquipmentID = equipment.EquipmentID,
                    Name = equipment.Name,
                    Status = equipment.Status,
                    Type = equipment.Type,
                    CreatedAt = DateTime.UtcNow
                };
                return Ok(equipmentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentDto createEquipmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var equipment = new Equipment
                {
                    Name = createEquipmentDto.Name,
                    Status = createEquipmentDto.Status,
                    Type = createEquipmentDto.Type
                };
                _connection.Equipment.Add(equipment);
                await _connection.SaveChangesAsync();
                return Ok(new { message = "Оборудование создано" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipment(int id, [FromBody] UpdateEquipmentDto updateEquipmentDto)
        {
            try
            {
                var equipment = await _connection.Equipment.FindAsync(id);
                if (equipment == null)
                {
                    return NotFound(new { message = "Оборудование не найдено" });
                }

                equipment.Name = updateEquipmentDto.Name;
                equipment.Status = updateEquipmentDto.Status;
                equipment.Type = updateEquipmentDto.Type;

                _connection.Equipment.Update(equipment);
                await _connection.SaveChangesAsync();
                return Ok(new { message = "Оборудование обновлено" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            try
            {
                var equipment = await _connection.Equipment.FindAsync(id);
                if (equipment == null)
                {
                    return NotFound(new { message = "Оборудование не найдено" });
                }

                _connection.Equipment.Remove(equipment);
                await _connection.SaveChangesAsync();
                return Ok(new { message = "Оборудование удалено" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка сервера: {ex.Message}" });
            }
        }
    }
}
