using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Domain.Entities
{
    public class EventDate
    {
        [Key]
        public Guid EventDateId { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        public List<EventTimeSlot> TimeSlots { get; set; } = new();
    }
}
