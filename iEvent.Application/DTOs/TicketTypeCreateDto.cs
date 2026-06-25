using iEvent.Domain.Enums;
using System;

namespace iEvent.Application.DTOs
{
    public class TicketTypeCreateDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
        public TicketIcon Icon { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableUntil { get; set; }
    }
}
