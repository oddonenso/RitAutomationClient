using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using SuperServerRIT.Commands;
using Microsoft.AspNetCore.JsonPatch;
using Data.Tables;

namespace SuperServerRIT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EquipmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Создание оборудования
        [HttpPost("create")]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { message = result });
        }

        // Обновление оборудования
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateEquipment(int id, [FromBody] JsonPatchDocument<Equipment> patch)
        {
            var command = new UpdateEquipmentCommand { EquipmentId = id, PatchDocument = patch };
            var result = await _mediator.Send(command);
            return Ok(new { message = result });
        }

        // Удаление оборудования
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            var command = new DeleteEquipmentCommand { EquipmentId = id };
            var result = await _mediator.Send(command);
            return Ok(new { message = result });
        }

        // Получение списка оборудования
        [HttpGet("list")]
        public async Task<IActionResult> GetEquipmentList()
        {
            var query = new GetAllEquipmentCommand();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
