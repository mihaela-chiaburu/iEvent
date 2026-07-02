namespace iEvent.Application.DTOs.Booking
{
    public class BookingTicketCreateDto
    {
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}
