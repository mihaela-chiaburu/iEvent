namespace iEvent.Application.DTOs.Booking
{
    public class BookingCollectAtVenueRespDto : BookingRespDto
    {
        public Guid CollectedById { get; set; }
        public double CollectedAmount { get; set; }
    }
}
