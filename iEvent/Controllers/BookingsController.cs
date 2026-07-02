using iEvent.Application.DTOs.Booking;
using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Payment;
using iEvent.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            return Ok(booking);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookingRespDto>> Create([FromBody] BookingCreateDto dto)
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (identityUserId == null) return Unauthorized();

            var booking = await _bookingService.CreateAsync(dto, identityUserId);
            return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, booking);
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPost("by-manager")]
        public async Task<ActionResult<BookingRespDto>> CreateByManager([FromBody] BookingByManagerDto dto)
        {
            var booking = await _bookingService.CreateByManagerAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, booking);
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookingUpdateDto dto)
        {
            await _bookingService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _bookingService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpGet("code/{code}")]
        public async Task<ActionResult<BookingRespDto>> GetByCode(string code)
        {
            var booking = await _bookingService.GetByCodeAsync(code);
            return Ok(booking);
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/mark-paid")]
        public async Task<IActionResult> MarkPaid(Guid id)
        {
            await _bookingService.MarkPaidAsync(id);

            return Ok(new { message = "Booking marked as paid." });
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/mark-unpaid")]
        public async Task<IActionResult> MarkUnpaid(Guid id)
        {
            await _bookingService.MarkUnpaidAsync(id);

            return Ok(new { message = "Booking marked as pending." });
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _bookingService.CancelAsync(id);

            return Ok(new { message = "Booking cancelled." });
        }

        [Authorize]
        [HttpPost("{id:guid}/simulate-payment")]
        public async Task<ActionResult<PaymentSimulationRespDto>> SimulatePayment( Guid id, [FromBody] SimulatePaymentRequestDto request)
        {
            var result = await _bookingService.SimulatePaymentAsync(
                id,
                request.ShouldSucceed);

            return Ok(result);
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/tickets/{ticketId:guid}")]
        public async Task<IActionResult> UpdateTicketQuantity(Guid id, Guid ticketId, [FromBody] BookingTicketUpdateQuantityDto dto)
        {
            await _bookingService.UpdateTicketQuantityAsync(id, ticketId, dto.NewQuantity);

            return NoContent();
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPost("{id:guid}/tickets")]
        public async Task<IActionResult> AddTicket(Guid id, [FromBody] BookingTicketAddDto dto)
        {
            await _bookingService.AddTicketToBookingAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "BookingManager,SuperAdmin")]
        [HttpPatch("{id:guid}/collect-at-venue")]
        public async Task<ActionResult<BookingCollectAtVenueRespDto>> CollectAtVenue(Guid id, [FromBody] BookingCollectAtVenueDto dto)
        {
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (managerId == null) return Unauthorized();

            var result = await _bookingService.CollectAtVenueAsync(id, dto, managerId);
            return Ok(result);
        }

        [Authorize] 
        [HttpGet("{id:guid}/qr")]
        public async Task<ActionResult<BookingQrCodeRespDto>> GetBookingQrCode(Guid id)
        {
            var result = await _bookingService.GetQrCodeAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id:guid}/pdf")]
        public async Task<IActionResult> GetTicketPdf(Guid id)
        {
            var pdfBytes = await _bookingService.GenerateTicketPdfAsync(id);
            return File(pdfBytes, "application/pdf", $"Ticket-{id}.pdf");
        }
    }
}
