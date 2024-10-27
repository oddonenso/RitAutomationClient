// Controllers/AuditLogController.cs
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SuperServerRIT.Commands;
using System.Threading.Tasks;

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuditLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/auditlog
        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] AddAuditLogCommand command)
        {
            if (command == null || command.UserID <= 0 || string.IsNullOrWhiteSpace(command.Action))
            {
                return BadRequest("Неверные данные для записи аудита.");
            }

            var logId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetLogById), new { id = logId }, command);
        }

        // GET: api/auditlog/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogById(int id)
        {
            var command = new GetAuditLogByIdCommand { LogID = id }; // Убедитесь, что такая команда существует
            var log = await _mediator.Send(command);

            return log != null ? Ok(log) : NotFound("Запись аудита не найдена.");
        }

        // GET: api/auditlog
        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var command = new GetAllAuditLogsCommand(); // Убедитесь, что такая команда существует
            var logs = await _mediator.Send(command);
            return Ok(logs);
        }

        // PUT: api/auditlog/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLog(int id, [FromBody] UpdateAuditLogCommand command)
        {
            if (command == null || command.LogID != id)
            {
                return BadRequest("Неверные данные для обновления записи аудита.");
            }

            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound("Запись аудита не найдена.");
        }

        // PATCH: api/auditlog/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchLog(int id, [FromBody] JsonPatchDocument<UpdateAuditLogCommand> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Запрос не может быть пустым.");
            }

            var command = new UpdateAuditLogCommand { LogID = id }; // Убедитесь, что такая команда существует
            patchDoc.ApplyTo(command);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound("Запись аудита не найдена.");
        }

        // DELETE: api/auditlog/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var command = new DeleteAuditLogCommand { LogID = id }; // Убедитесь, что такая команда существует
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound("Запись аудита не найдена.");
        }
    }
}
