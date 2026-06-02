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

        [Range(0, 1000000)]
        public double Price { get; set; }

        [Range(0, 1000000)]
        public int QuantityAvailable { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        public List<BookingTicket> Bookings { get; set; } = new();
    }
}
