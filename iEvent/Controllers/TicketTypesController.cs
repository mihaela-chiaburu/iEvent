using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/ticket-types")]
    public class TicketTypesController : ControllerBase
    {
        private readonly ITicketTypeService _ticketTypeService;

        public TicketTypesController(ITicketTypeService ticketTypeService)
        {
            _ticketTypeService = ticketTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketTypeRespDto>>> GetAll()
        {
            var ticketTypes = await _ticketTypeService.GetAllAsync();
            return Ok(ticketTypes);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TicketTypeRespDto>> GetById(Guid id)
        {
            var ticketType = await _ticketTypeService.GetByIdAsync(id);
            if (ticketType == null)
            {
                return NotFound();
            }

            return Ok(ticketType);
        }

        [HttpPost]
        public async Task<ActionResult<TicketTypeRespDto>> Create([FromBody] TicketTypeCreateDto dto)
        {
            var created = await _ticketTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.TicketTypeId }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TicketTypeUpdateDto dto)
        {
            var updated = await _ticketTypeService.UpdateAsync(id, dto);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _ticketTypeService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
