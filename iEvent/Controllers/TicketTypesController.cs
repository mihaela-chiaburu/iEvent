using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<TicketTypeRespDto>>> GetAll([FromQuery] Guid? EventId)
        {
            var ticketTypes = await _ticketTypeService.GetAllAsync(EventId);
            return Ok(ticketTypes);
        }

        [AllowAnonymous]
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

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<TicketTypeRespDto>> Create([FromBody] TicketTypeCreateDto dto)
        {
            var created = await _ticketTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.TicketTypeId }, created);
        }

        [Authorize(Roles = "EventManager,BookingManager,SuperAdmin")]
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

        [Authorize(Roles = "EventManager,SuperAdmin")]
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
