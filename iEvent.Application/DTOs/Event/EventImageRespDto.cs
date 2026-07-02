namespace iEvent.Application.DTOs.Event
{
    public class EventImageRespDto
    {
        public Guid ImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
