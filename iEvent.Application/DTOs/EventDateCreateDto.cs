using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class EventDateCreateDto
    {
        public DateOnly Date { get; set; }

        public List<EventTimeSlotCreateDto> TimeSlots { get; set; } = new();
    }
}
