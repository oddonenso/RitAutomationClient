﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SuperServerRIT.Commands;
using Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using SuperServerRIT.Model;

namespace SuperServerRIT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EquipmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Создает новое оборудование.
        /// </summary>
        /// <param name="command">Команда для создания оборудования.</param>
        /// <returns>Результат операции создания оборудования.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.Name) || string.IsNullOrWhiteSpace(command.Type))
            {
                return BadRequest("Invalid equipment data.");
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Получает все оборудование.
        /// </summary>
        /// <returns>Список всего оборудования.</returns>
        [HttpGet]
        public async Task<ActionResult<List<Equipment>>> GetAllEquipment()
        {
            var result = await _mediator.Send(new GetAllEquipmentCommand());
            return Ok(result);
        }

        /// <summary>
        /// Удаляет оборудование по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор оборудования для удаления.</param>
        /// <returns>Результат операции удаления оборудования.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            var command = new DeleteEquipmentCommand { EquipmentId = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Обновляет данные оборудования.
        /// </summary>
        /// <param name="id">Идентификатор оборудования для обновления.</param>
        /// <param name="updateDto">Данные для обновления оборудования.</param>
        /// <returns>Результат операции обновления оборудования.</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateEquipment(int id, [FromBody] UpdateEquipmentDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Invalid update data.");
            }

            var command = new UpdateEquipmentCommand
            {
                EquipmentId = id,
                Name = updateDto.Name,
                Status = updateDto.Status,
                Type = updateDto.Type
            };

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
