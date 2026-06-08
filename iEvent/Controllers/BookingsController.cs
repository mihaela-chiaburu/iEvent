using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iEvent.WebApi.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<List<BookingRespDto>>> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookingRespDto>> GetById(Guid id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookingRespDto>> Create([FromBody] BookingCreateDto dto)
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (identityUserId == null)
            {
                return Unauthorized();
            }

            var booking = await _bookingService.CreateAsync(dto, identityUserId);
            if (booking == null)
            {
                return BadRequest("Invalid ticket types.");
            }

            return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, booking);
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookingUpdateDto dto)
        {
            var updated = await _bookingService.UpdateAsync(id, dto);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _bookingService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpGet("code/{code}")]
        public async Task<ActionResult<BookingRespDto>> GetByCode(string code)
        {
            var booking = await _bookingService.GetByCodeAsync(code);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }
    }
}
