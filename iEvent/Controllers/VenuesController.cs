using iEvent.Application.DTOs.Event;
using iEvent.Application.DTOs.Venue;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/venues")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;
        private readonly IEventService _eventService;

        public VenuesController(IVenueService venueService, IEventService eventService)
        {
            _venueService = venueService;
            _eventService = eventService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<VenueRespDto>>> GetAll()
        {
            var venues = await _venueService.GetAllAsync();
            return Ok(venues);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VenueRespDto>> GetById(Guid id)
        {
            var venue = await _venueService.GetByIdAsync(id);
            return Ok(venue);
        }

        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<ActionResult<List<VenueRespDto>>> GetPopular()
        {
            var venues = await _venueService.GetPopularAsync(10);
            return Ok(venues);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<VenueRespDto>> Create([FromBody] VenueCreateDto dto)
        {
            var created = await _venueService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.VenueId }, created);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VenueUpdateDto dto)
        {
            await _venueService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _venueService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost("draft")]
        public async Task<ActionResult<VenueRespDto>> CreateDraft()
        {
            var created = await _venueService.CreateDraftAsync();
            return Ok(created);
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] VenuePatchDto dto)
        {
            await _venueService.PatchAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost("{id:guid}/publish")]
        public async Task<IActionResult> Publish(Guid id)
        {
            await _venueService.PublishAsync(id);
            return NoContent();
        }

        [HttpGet("{id:guid}/events")]
        [AllowAnonymous]
        public async Task<ActionResult<List<EventRespDto>>> GetEvents(Guid id)
        {
            var events = await _eventService.GetEventsByVenueIdAsync(id);
            return Ok(events);
        }
    }
}
