using Data;
using Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperServerRIT.Model;

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly Connection _connection;

        public NotificationController(Connection connection)
        {
            _connection = connection;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotif()
        {
            var notif = await _connection.Notification.Include(x => x.Equipment).ToListAsync();

            var result = notif.Select(x => new NotificationDto
            {
                EquipmentId = x.EquipmentID,
                Message = x.Message,
                Timestamp = x.Timestamp
            }).ToList();
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var notification = await _connection.Notification
                .Include(n => n.Equipment)
                .FirstOrDefaultAsync(n => n.NotificationID == id);

            if (notification == null)
            {
                return NotFound(new { message = "Уведомление не найдено" });
            }

            var notificationDto = new NotificationDto
            {
                EquipmentId = notification.EquipmentID,
                Message = notification.Message,
                Timestamp = notification.Timestamp
            };

            return Ok(notificationDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationDto notificationDto)
        {
            var equipment = await _connection.Equipment.FirstOrDefaultAsync(e => e.EquipmentID == notificationDto.EquipmentId);
            if (equipment == null)
            {
                return BadRequest(new { message = "Оборудование не найдено" });
            }

            var notification = new Notification
            {
                EquipmentID = notificationDto.EquipmentId,
                Message = notificationDto.Message,
                Timestamp = DateTime.UtcNow
            };

            _connection.Notification.Add(notification);
            await _connection.SaveChangesAsync();

            return Ok(notification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] NotificationDto notificationDto)
        {
            var notification = await _connection.Notification.FirstOrDefaultAsync(n => n.NotificationID == id);
            if (notification == null)
            {
                return NotFound(new { message = "Уведомление не найдено" });
            }

            notification.Message = notificationDto.Message;
            notification.Timestamp = DateTime.UtcNow;
            notification.EquipmentID = notificationDto.EquipmentId;

            await _connection.SaveChangesAsync();
            return Ok(notification);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _connection.Notification.FirstOrDefaultAsync(n => n.NotificationID == id);
            if (notification == null)
            {
                return NotFound(new { message = "Уведомление не найдено" });
            }

            _connection.Notification.Remove(notification);
            await _connection.SaveChangesAsync();

            return Ok(new { message = "Уведомление удалено" });
        }
    }
}
