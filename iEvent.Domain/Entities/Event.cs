using iEvent.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iEvent.Domain.Entities
{
    public class Event
    {
        [Key]
        public Guid EventId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public Guid? VenueId { get; set; }

        [ForeignKey(nameof(VenueId))]
        public Venue? Venue { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public List<TicketType> Tickets { get; set; } = new();

        [Required]
        public EventCategory Category { get; set; } = EventCategory.Other;

        public List<EventDate> EventDates { get; set; } = new();

        public List<EventImage> Images { get; set; } = new();
        public bool IsDraft { get; set; } = true;
    }
}
