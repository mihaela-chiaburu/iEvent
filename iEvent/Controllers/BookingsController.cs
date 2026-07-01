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
        public async Task<ActionResult<PagedResultDto<BookingRespDto>>> GetAll([FromQuery] BookingFilterDto filter)
        {
            var pagedResult = await _bookingService.GetAllAsync(filter);
            return Ok(pagedResult);
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
        [HttpPost("by-manager")]
        public async Task<ActionResult<BookingRespDto>> CreateByManager([FromBody] BookingByManagerDto dto)
        {
            try
            {
                var booking = await _bookingService.CreateByManagerAsync(dto);

                if (booking == null)
                {
                    return BadRequest("Invalid TimeSlot or Event combination.");
                }

                return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, booking);
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

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPost("{id:guid}/tickets")]
        public async Task<IActionResult> AddTicket(Guid id, [FromBody] BookingTicketAddDto dto)
        {
            try
            {
                var success = await _bookingService.AddTicketToBookingAsync(id, dto);
                if (!success)
                {
                    return BadRequest("Could not add ticket to booking.");
                }

                return NoContent(); 
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
        [HttpPatch("{id:guid}/collect-at-venue")]
        public async Task<ActionResult<BookingCollectAtVenueRespDto>> CollectAtVenue(Guid id, [FromBody] BookingCollectAtVenueDto dto)
        {
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (managerId == null) return Unauthorized();

            try
            {
                var result = await _bookingService.CollectAtVenueAsync(id, dto, managerId);
                if (result == null) return NotFound($"Booking with ID {id} was not found.");

                return Ok(result);
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize] 
        [HttpGet("{id:guid}/qr")]
        public async Task<ActionResult<BookingQrCodeRespDto>> GetBookingQrCode(Guid id)
        {
            var result = await _bookingService.GetQrCodeAsync(id);

            if (result == null)
            {
                return NotFound($"Booking with ID {id} was not found.");
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id:guid}/pdf")]
        public async Task<IActionResult> GetTicketPdf(Guid id)
        {
            var pdfBytes = await _bookingService.GenerateTicketPdfAsync(id);

            if (pdfBytes == null)
            {
                return NotFound($"Booking with ID {id} was not found.");
            }

            return File(pdfBytes, "application/pdf", $"Ticket-{id}.pdf");
        }
    }
}
