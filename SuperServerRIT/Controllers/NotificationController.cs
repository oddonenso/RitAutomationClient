using Microsoft.AspNetCore.Mvc;
using MediatR;
using SuperServerRIT.Commands;
using SuperServerRIT.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает все уведомления.
        /// </summary>
        /// <returns>Список всех уведомлений.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var command = new GetAllNotificationsCommand();
            var notifications = await _mediator.Send(command);
            return Ok(notifications);
        }

        /// <summary>
        /// Получает уведомление по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор уведомления.</param>
        /// <returns>Уведомление с указанным идентификатором.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var command = new GetNotificationByIdCommand(id);
            var notification = await _mediator.Send(command);

            if (notification == null)
            {
                return NotFound(new { message = "Уведомление не найдено" });
            }

            return Ok(notification);
        }

        /// <summary>
        /// Добавляет новое уведомление.
        /// </summary>
        /// <param name="command">Команда для добавления уведомления.</param>
        /// <returns>Созданное уведомление.</returns>
        [HttpPost]
        public async Task<IActionResult> AddNotification([FromBody] AddNotificationCommand command)
        {
            if (command == null)
            {
                return BadRequest("Некорректные данные для уведомления.");
            }

            var notificationId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetNotificationById), new { id = notificationId }, command);
        }

        /// <summary>
        /// Частично обновляет уведомление.
        /// </summary>
        /// <param name="id">Идентификатор уведомления для обновления.</param>
        /// <param name="command">Команда для обновления уведомления.</param>
        /// <returns>Результат операции обновления.</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchNotification(int id, [FromBody] UpdateNotificationCommand command)
        {
            if (command.NotificationID != id)
            {
                return BadRequest("ID в теле запроса не совпадает с ID в URL.");
            }

            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new { message = "Уведомление не найдено" });
            }

            return NoContent();
        }

        /// <summary>
        /// Удаляет уведомление по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор уведомления для удаления.</param>
        /// <returns>Результат операции удаления.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var command = new DeleteNotificationCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new { message = "Уведомление не найдено" });
            }

            return NoContent();
        }
    }
}
