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

        /// <summary>
        /// Создает новую запись аудита.
        /// </summary>
        /// <param name="command">Команда для создания записи аудита.</param>
        /// <returns>Созданную запись аудита.</returns>
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

        /// <summary>
        /// Получает запись аудита по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор записи аудита.</param>
        /// <returns>Запись аудита с указанным идентификатором.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogById(int id)
        {
            var command = new GetAuditLogByIdCommand { LogID = id };
            var log = await _mediator.Send(command);

            return log != null ? Ok(log) : NotFound("Запись аудита не найдена.");
        }

        /// <summary>
        /// Получает все записи аудита.
        /// </summary>
        /// <returns>Список всех записей аудита.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var command = new GetAllAuditLogsCommand();
            var logs = await _mediator.Send(command);
            return Ok(logs);
        }

        /// <summary>
        /// Обновляет запись аудита.
        /// </summary>
        /// <param name="id">Идентификатор записи аудита для обновления.</param>
        /// <param name="command">Команда для обновления записи аудита.</param>
        /// <returns>Результат операции обновления.</returns>
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

        /// <summary>
        /// Частично обновляет запись аудита.
        /// </summary>
        /// <param name="id">Идентификатор записи аудита для обновления.</param>
        /// <param name="patchDoc">Документ для частичного обновления.</param>
        /// <returns>Результат операции частичного обновления.</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchLog(int id, [FromBody] JsonPatchDocument<UpdateAuditLogCommand> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Запрос не может быть пустым.");
            }

            var command = new UpdateAuditLogCommand { LogID = id };
            patchDoc.ApplyTo(command);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound("Запись аудита не найдена.");
        }

        /// <summary>
        /// Удаляет запись аудита.
        /// </summary>
        /// <param name="id">Идентификатор записи аудита для удаления.</param>
        /// <returns>Результат операции удаления.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var command = new DeleteAuditLogCommand { LogID = id };
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound("Запись аудита не найдена.");
        }
    }
}
