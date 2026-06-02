using System;

namespace iEvent.Application.DTOs
{
    public class BookingTicketCreateDto
    {
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}
