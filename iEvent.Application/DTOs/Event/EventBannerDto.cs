namespace iEvent.Application.DTOs.Event
{
    public class EventBannerDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
