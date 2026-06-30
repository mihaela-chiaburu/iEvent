using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Enums;
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

        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<List<BookingRespDto>>> GetMyBookings()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (identityUserId == null)
            {
                return Unauthorized();
            }

            var bookings = await _bookingService.GetMyBookingsAsync(identityUserId);

            return Ok(bookings);
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
            if (identityUserId == null) return Unauthorized();

            try
            {
                var booking = await _bookingService.CreateAsync(dto, identityUserId);
                return CreatedAtAction(nameof(GetById), new { id = booking!.BookingId }, booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/mark-paid")]
        public async Task<IActionResult> MarkPaid(Guid id)
        {
            var success = await _bookingService.MarkPaidAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return Ok(new { message = "Booking marked as paid." });
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/mark-unpaid")]
        public async Task<IActionResult> MarkUnpaid(Guid id)
        {
            var success = await _bookingService.MarkUnpaidAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return Ok(new { message = "Booking marked as pending." });
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var success = await _bookingService.CancelAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return Ok(new { message = "Booking cancelled." });
        }

        [Authorize]
        [HttpPost("{id:guid}/simulate-payment")]
        public async Task<ActionResult<PaymentSimulationRespDto>> SimulatePayment( Guid id, [FromBody] SimulatePaymentRequestDto request)
        {
            var result = await _bookingService.SimulatePaymentAsync(
                id,
                request.ShouldSucceed);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/tickets/{ticketId:guid}")]
        public async Task<IActionResult> UpdateTicketQuantity(Guid id, Guid ticketId, [FromBody] BookingTicketUpdateQuantityDto dto)
        {
            var success = await _bookingService.UpdateTicketQuantityAsync(id, ticketId, dto.NewQuantity);

            if (!success)
            {
                return NotFound("Booking or Ticket not found.");
            }

            return NoContent();
        }
    }
}
