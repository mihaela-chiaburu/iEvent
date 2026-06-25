using iEvent.Domain.Enums;
using System;

namespace iEvent.Application.DTOs
{
    public class EventCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EventDateCreateDto> EventDates { get; set; } = new();
        public Guid VenueId { get; set; }
        public string? ImageUrl { get; set; }
        public EventCategory Category { get; set; }
        public List<EventImageCreateDto> Images { get; set; } = new();
    }
}
