namespace iEvent.Application.DTOs.Event
{
    public class EventDateRespDto
    {
        public Guid EventDateId { get; set; }
        public DateOnly Date { get; set; }
        public List<EventTimeSlotRespDto> TimeSlots { get; set; } = new();
    }
}
