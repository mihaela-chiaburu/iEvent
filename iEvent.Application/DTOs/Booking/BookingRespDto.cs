using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs.Booking
{
    public class BookingRespDto
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid EventId { get; set; }
        public Guid BookingTimeSlotId { get; set; }
        public double AdminFee { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public double TotalPrice { get; set; }
        public List<BookingTicketRespDto> Tickets { get; set; } = new();
        public string BookingCode { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime? PaidAt { get; set; }

    }
}
