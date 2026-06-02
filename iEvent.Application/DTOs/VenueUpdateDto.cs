namespace iEvent.Application.DTOs
{
    public class VenueUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
