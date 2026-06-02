using System;

namespace iEvent.Application.DTOs
{
    public class TicketTypeUpdateDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int QuantityAvailable { get; set; }
    }
}
