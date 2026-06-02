using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iEvent.Domain.Entities
{
    public class BookingTicket
    {
        [Key]
        public Guid BookingTicketId { get; set; }

        [Required]
        public Guid BookingId { get; set; }

        [Required]
        public Guid TicketTypeId { get; set; }

        [Range(1, 1000000)]
        public int Quantity { get; set; }

        [Range(0, 1000000)]
        public decimal UnitPrice { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        [ForeignKey(nameof(TicketTypeId))]
        public TicketType? TicketType { get; set; }
    }
}
