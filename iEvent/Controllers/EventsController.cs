using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Services;
using iEvent.Application.Services;
using iEvent.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ITicketTypeService _ticketTypeService;
        public EventsController(IEventService eventService, ITicketTypeService ticketTypeService)
        {
            _eventService = eventService;
            _ticketTypeService = ticketTypeService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<EventRespDto>>> GetAll(
            [FromQuery] string? city,
            [FromQuery] Guid? venueId,
            [FromQuery] EventCategory? category,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var events = await _eventService.GetAllAsync(city, venueId, category, fromDate, toDate);
            return Ok(events);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EventRespDto>> GetById(Guid id)
        {
            var ievent = await _eventService.GetByIdAsync(id);
            if (ievent == null)
            {
                return NotFound();
            }

            return Ok(ievent);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<EventRespDto>> Create([FromBody] EventCreateDto dto)
        {
            var created = await _eventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.EventId }, created);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EventUpdateDto dto)
        {
            var updated = await _eventService.UpdateAsync(id, dto);
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
            var deleted = await _eventService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}/ticket-types")]
        public async Task<ActionResult<List<TicketTypeRespDto>>> GetTicketTypes(Guid id)
        {
            var ticketTypes = await _ticketTypeService.GetAllAsync(id);

            return Ok(ticketTypes);
        }


    }
}
