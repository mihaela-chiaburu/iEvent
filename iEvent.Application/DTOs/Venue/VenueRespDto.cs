namespace iEvent.Application.DTOs.Venue
{
    public class VenueRespDto
    {
        public Guid VenueId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int EventCount { get; set; }
        public List<VenueFacilityRespDto> Facilities { get; set; } = new();
        public List<VenueImageRespDto> Images { get; set; } = new();
    }
}
