using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs.Event
{
    public class EventRespDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EventDateRespDto> EventDates { get; set; } = new();
        public Guid? VenueId { get; set; }
        public EventCategory Category { get; set; }
        public List<EventImageRespDto> Images { get; set; } = new();
        public decimal MinTicketPrice { get; set; }
        public List<DateOnly> AllDates { get; set; } = new();
    }
}
