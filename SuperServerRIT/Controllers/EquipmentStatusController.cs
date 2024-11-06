using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SuperServerRIT.Commands;
using System.Threading.Tasks;

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentStatusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EquipmentStatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Добавляет новый статус оборудования.
        /// </summary>
        /// <param name="command">Команда для добавления статуса оборудования.</param>
        /// <returns>Созданный статус оборудования.</returns>
        [HttpPost]
        public async Task<IActionResult> AddEquipmentStatus([FromBody] AddEquipmentStatusCommand command)
        {
            if (command == null || command.EquipmentID <= 0 ||
                string.IsNullOrWhiteSpace(command.Location))
            {
                return BadRequest("Неверные данные статуса оборудования.");
            }

            var equipmentStatusId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetEquipmentStatusById), new { id = equipmentStatusId }, command);
        }

        /// <summary>
        /// Получает статус оборудования по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор статуса оборудования.</param>
        /// <returns>Статус оборудования с указанным идентификатором.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipmentStatusById(int id)
        {
            var command = new GetEquipmentStatusByIdCommand { EquipmentStatusID = id };
            var equipmentStatus = await _mediator.Send(command);

            if (equipmentStatus == null)
            {
                return NotFound("Статус оборудования не найден.");
            }

            return Ok(equipmentStatus);
        }

        /// <summary>
        /// Обновляет статус оборудования.
        /// </summary>
        /// <param name="id">Идентификатор статуса оборудования для обновления.</param>
        /// <param name="patchDoc">Документ для частичного обновления статуса.</param>
        /// <returns>Результат операции обновления статуса оборудования.</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateEquipmentStatus(int id, [FromBody] JsonPatchDocument<UpdateEquipmentStatusCommand> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Запрос не может быть пустым.");
            }

            var command = new UpdateEquipmentStatusCommand { EquipmentStatusID = id };
            patchDoc.ApplyTo(command);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            if (!result)
            {
                return NotFound("Статус оборудования не найден.");
            }

            return NoContent();
        }

        /// <summary>
        /// Удаляет статус оборудования.
        /// </summary>
        /// <param name="id">Идентификатор статуса оборудования для удаления.</param>
        /// <returns>Результат операции удаления статуса оборудования.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipmentStatus(int id)
        {
            var command = new DeleteEquipmentStatusCommand { EquipmentStatusID = id };
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound("Статус оборудования не найден.");
            }

            return NoContent();
        }
    }
}
