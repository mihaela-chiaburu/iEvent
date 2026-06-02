using System;

namespace iEvent.Application.DTOs
{
    public class EventCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid VenueId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
