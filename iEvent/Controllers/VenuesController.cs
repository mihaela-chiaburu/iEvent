using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.Controllers
{
    [ApiController]
    [Route("api/venues")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        public async Task<ActionResult<List<VenueRespDto>>> GetAll()
        {
            var venues = await _venueService.GetAllAsync();
            return Ok(venues);
        }

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

        [HttpPost]
        public async Task<ActionResult<VenueRespDto>> Create([FromBody] VenueCreateDto dto)
        {
            var created = await _venueService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.VenueId }, created);
        }

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
    }
}
