namespace iEvent.Application.DTOs.Booking
{
    public class BookingTicketRespDto
    {
        public Guid BookingTicketId { get; set; }
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
