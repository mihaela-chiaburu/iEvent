using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs.Booking
{
    public class BookingCreateDto
    {
        public Guid EventId { get; set; }
        public Guid BookingTimeSlotId { get; set; }
        public List<BookingTicketCreateDto> Tickets { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; }
    }
}
