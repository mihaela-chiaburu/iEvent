using iEvent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs.Event
{
    public class EventPatchDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? VenueId { get; set; }
        public string? ImageUrl { get; set; }

        public List<EventDateCreateDto> EventDates { get; set; } = new();
    }
}
