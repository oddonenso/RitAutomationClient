
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

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var command = new GetAllNotificationsCommand();
            var notifications = await _mediator.Send(command);
            return Ok(notifications);
        }

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

        [HttpPost]
        public async Task<IActionResult> AddNotification([FromBody] AddNotificationCommand command)
        {
            var notificationId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetNotificationById), new { id = notificationId }, command);
        }

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
