namespace iEvent.Application.DTOs.Event
{
    public class EventTimeSlotCreateDto
    {
        public TimeOnly StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }
    }
}
