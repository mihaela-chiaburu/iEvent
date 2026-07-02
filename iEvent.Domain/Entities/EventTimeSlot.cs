using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iEvent.Domain.Entities
{
    public class EventTimeSlot
    {
        [Key]
        public Guid TimeSlotId { get; set; }

        [Required]
        public Guid EventDateId { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        [ForeignKey(nameof(EventDateId))]
        public EventDate? EventDate { get; set; }
    }
}
