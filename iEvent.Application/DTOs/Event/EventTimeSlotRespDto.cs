namespace iEvent.Application.DTOs.Event
{
    public class EventTimeSlotRespDto
    {
        public Guid TimeSlotId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}
