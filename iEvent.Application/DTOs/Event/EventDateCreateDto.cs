namespace iEvent.Application.DTOs.Event
{
    public class EventDateCreateDto
    {
        public DateOnly Date { get; set; }
        public List<EventTimeSlotCreateDto> TimeSlots { get; set; } = new();
    }
}
