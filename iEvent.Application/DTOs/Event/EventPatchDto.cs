using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs.Event
{
    public class EventPatchDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EventDateCreateDto> EventDates { get; set; } = new();
        public Guid VenueId { get; set; }
        public EventCategory Category { get; set; }
        public List<EventImageCreateDto> Images { get; set; } = new();
    }
}
