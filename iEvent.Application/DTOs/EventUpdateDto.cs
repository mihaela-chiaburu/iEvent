using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs
{
    public class EventUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EventDateCreateDto> EventDates { get; set; } = new();
        public Guid VenueId { get; set; }
        public string? ImageUrl { get; set; }
        public EventCategory Category { get; set; }
    }
}
