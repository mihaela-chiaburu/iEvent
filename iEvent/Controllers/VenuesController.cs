using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Services;
using iEvent.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iEvent.Controllers
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
            if (venue == null)
            {
                return NotFound();
            }

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
            var updated = await _venueService.UpdateAsync(id, dto);
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
            var deleted = await _venueService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

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
            var updated = await _venueService.PatchAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "EventManager,SuperAdmin")]
        [HttpPost("{id:guid}/publish")]
        public async Task<IActionResult> Publish(Guid id)
        {
            var result = await _venueService.PublishAsync(id);

            if (!result)
                return NotFound();

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
