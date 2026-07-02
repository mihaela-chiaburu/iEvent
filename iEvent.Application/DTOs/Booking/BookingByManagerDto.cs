using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs.Booking
{
    public class BookingByManagerDto
    {
        public Guid EventId { get; set; }
        public Guid BookingTimeSlotId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<BookingManagerTicketDto> Tickets { get; set; } = new();
        public Guid? CustomerId { get; set; }
        public NewCustomerDto? NewCustomer { get; set; }
    }
}
