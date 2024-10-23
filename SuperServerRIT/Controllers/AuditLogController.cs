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
    public class AuditLogController : Controller
    {
        private readonly Connection _connection;

        public AuditLogController(Connection connection)
        {
            _connection = connection;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] AuditLogDto auditLogDto)
        {
            try
            {
                var user = await _connection.Users.FirstOrDefaultAsync(x => x.UserID == auditLogDto.UserID);
                if (user == null)
                {
                    return BadRequest(new { message = "Пользователь не найден" });
                }
                var log = new AuditLog
                {
                    UserID = auditLogDto.UserID,
                    Action = auditLogDto.Action,
                    EntityAffected = auditLogDto.EntityAffected,
                    EntityID = auditLogDto.EntityID,
                    Timestamp = DateTime.UtcNow
                };

                _connection.AuditLog.Add(log);
                await _connection.SaveChangesAsync();
                return Ok(log);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при создании лога", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogById(int id)
        {
            try
            {
                var log = await _connection.AuditLog
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.LogID == id);

                if (log == null)
                {
                    return NotFound(new { message = "Запись аудита не найдена" });
                }

                var logDto = new AuditLogDto
                {
                    UserID = log.UserID,
                    Action = log.Action,
                    EntityAffected = log.EntityAffected,
                    EntityID = log.EntityID,
                    Timestamp = log.Timestamp
                };

                return Ok(logDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при получении лога", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _connection.AuditLog
                    .Include(l => l.User) // Загрузить данные пользователя
                    .ToListAsync();

                var result = logs.Select(l => new AuditLogDto
                {
                    UserID = l.UserID,
                    Action = l.Action,
                    EntityAffected = l.EntityAffected,
                    EntityID = l.EntityID,
                    Timestamp = l.Timestamp
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при получении логов", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLog(int id, [FromBody] AuditLogDto auditLogDto)
        {
            try
            {
                var log = await _connection.AuditLog.FirstOrDefaultAsync(l => l.LogID == id);
                if (log == null)
                {
                    return NotFound(new { message = "Запись аудита не найдена" });
                }

                log.Action = auditLogDto.Action;
                log.EntityAffected = auditLogDto.EntityAffected;
                log.EntityID = auditLogDto.EntityID;
                log.UserID = auditLogDto.UserID;
                log.Timestamp = DateTime.UtcNow;

                await _connection.SaveChangesAsync();
                return Ok(log);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при обновлении лога", error = ex.Message });
            }
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchLog(int id, [FromBody] JsonPatchDocument<AuditLog> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    return BadRequest(new { message = "Неверный Patch документ" });
                }

                var log = await _connection.AuditLog.FirstOrDefaultAsync(l => l.LogID == id);
                if (log == null)
                {
                    return NotFound(new { message = "Запись аудита не найдена" });
                }

                patchDoc.ApplyTo(log);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _connection.SaveChangesAsync();
                return Ok(new { message = "Запись аудита успешно обновлена" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при обновлении записи аудита: {ex.Message}" });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            try
            {
                var log = await _connection.AuditLog.FirstOrDefaultAsync(l => l.LogID == id);
                if (log == null)
                {
                    return NotFound(new { message = "Запись аудита не найдена" });
                }

                _connection.AuditLog.Remove(log);
                await _connection.SaveChangesAsync();

                return Ok(new { message = "Запись аудита удалена" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при удалении лога", error = ex.Message });
            }
        }
    }
}
