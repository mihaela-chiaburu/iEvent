namespace iEvent.Application.DTOs.Venue
{
    public class VenuePatchDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? Capacity { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public List<VenueFacilityCreateDto>? Facilities { get; set; }
    }
}
