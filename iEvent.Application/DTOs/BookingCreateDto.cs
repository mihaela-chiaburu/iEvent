using System;
using System.Collections.Generic;

namespace iEvent.Application.DTOs
{
    public class BookingCreateDto
    {
        public Guid CustomerId { get; set; }
        public Guid EventId { get; set; }
        public List<BookingTicketCreateDto> Tickets { get; set; } = new();
    }
}
