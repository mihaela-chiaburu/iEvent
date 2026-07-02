using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iEvent.Domain.Enums;

namespace iEvent.Domain.Entities
{
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Range(0, 10000000)]
        public decimal TotalPrice { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        public List<BookingTicket> BookingTickets { get; set; } = new();

        [Required]
        [StringLength(20)]
        public string BookingCode { get; set; } = string.Empty;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime? PaidAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public Guid BookingTimeSlotId { get; set; }

        [ForeignKey(nameof(BookingTimeSlotId))]
        public EventTimeSlot? BookingTimeSlot { get; set; }

        [Range(0, 10000000)]
        public decimal AdminFee { get; set; }

        public Guid? CollectedById { get; set; }

        [ForeignKey(nameof(CollectedById))]
        public AdminUser? CollectedBy { get; set; }

        [Range(0, 10000000)]
        public decimal? CollectedAmount { get; set; }
    }
}
