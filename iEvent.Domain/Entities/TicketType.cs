using iEvent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iEvent.Domain.Entities
{
    public class TicketType
    {
        [Key]
        public Guid TicketTypeId { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public TicketIcon Icon { get; set; } = TicketIcon.Standard;

        [Range(0, 1000000)]
        public decimal Price { get; set; }

        [Range(0, 1000000)]
        public int QuantityAvailable { get; set; }

        public DateTime? AvailableFrom { get; set; }

        public DateTime? AvailableUntil { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        public List<BookingTicket> Bookings { get; set; } = new();
    }
}
