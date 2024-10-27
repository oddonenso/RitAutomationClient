// Controllers/EquipmentStatusController.cs
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

        // POST: api/equipment/status
        [HttpPost]
        public async Task<IActionResult> AddEquipmentStatus([FromBody] AddEquipmentStatusCommand command)
        {
            if (command == null || command.EquipmentID <= 0 ||
                string.IsNullOrWhiteSpace(command.Location) ||
                string.IsNullOrWhiteSpace(command.Status))
            {
                return BadRequest("Неверные данные статуса оборудования.");
            }

            var equipmentStatusId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetEquipmentStatusById), new { id = equipmentStatusId }, command);
        }

        // GET: api/equipment/status/{id}
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
