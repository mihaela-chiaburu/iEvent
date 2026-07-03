namespace iEvent.Application.DTOs.Venue
{
    public class VenueImageRespDto
    {
        public Guid ImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
