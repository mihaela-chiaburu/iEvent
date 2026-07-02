using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs.Event
{
    public class EventTimeSlotCreateDto
    {
        public TimeOnly StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }
    }
}
