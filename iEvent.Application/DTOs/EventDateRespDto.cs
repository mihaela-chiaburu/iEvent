using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class EventDateRespDto
    {
        public Guid EventDateId { get; set; }
        public DateOnly Date { get; set; }
        public List<EventTimeSlotRespDto> TimeSlots { get; set; } = new();
    }
}
