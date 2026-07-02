using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Event;
using iEvent.Application.DTOs.Tickets;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
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
        public async Task<ActionResult<PagedResultDto<EventRespDto>>> GetAll([FromQuery] EventQueryDto query)
        {
            var pagedResult = await _eventService.GetAllAsync(query);
            return Ok(pagedResult);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EventRespDto>> GetById(Guid id)
        {
            var ievent = await _eventService.GetByIdAsync(id);
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
            await _eventService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _eventService.DeleteAsync(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}/ticket-types")]
        public async Task<ActionResult<List<TicketTypeRespDto>>> GetTicketTypes(Guid id)
        {
            var ticketTypes = await _ticketTypeService.GetAllAsync(id);

            return Ok(ticketTypes);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}/dates")]
        public async Task<ActionResult<List<EventDateRespDto>>> GetDates(Guid id)
        {
            var dates = await _eventService.GetEventDatesAsync(id);
            return Ok(dates);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost("draft")]
        public async Task<ActionResult<EventRespDto>> CreateDraft()
        {
            var created = await _eventService.CreateDraftAsync();
            return Ok(created);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] EventPatchDto dto)
        {
            await _eventService.PatchAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost("{id:guid}/publish")]
        public async Task<IActionResult> Publish(Guid id)
        {
            await _eventService.PublishAsync(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("popular")]
        public async Task<ActionResult<List<EventRespDto>>> GetPopular()
        {
            var popularEvents = await _eventService.GetPopularEventsAsync(4);
            return Ok(popularEvents);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost("{id:guid}/dates")]
        public async Task<IActionResult> AddDates(Guid id, [FromBody] List<EventDateCreateDto> dto)
        {
            await _eventService.AddEventDatesAsync(id, dto);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}/similar")]
        public async Task<ActionResult<List<EventRespDto>>> GetSimilar(Guid id)
        {
            var similarEvents = await _eventService.GetSimilarEventsAsync(id, 4);
            return Ok(similarEvents);
        }

        [AllowAnonymous]
        [HttpGet("by-category")]
        public async Task<ActionResult<List<EventRespDto>>> GetPreviewByCategory([FromQuery] string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return BadRequest("Category is required.");
            }

            var events = await _eventService.GetPreviewByCategoryAsync(category, 4);
            return Ok(events);
        }

        [AllowAnonymous]
        [HttpGet("by-city")]
        public async Task<ActionResult<List<EventRespDto>>> GetPreviewByCity([FromQuery] string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City is required.");
            }

            var events = await _eventService.GetPreviewByCityAsync(city, 4);
            return Ok(events);
        }

    }
}
