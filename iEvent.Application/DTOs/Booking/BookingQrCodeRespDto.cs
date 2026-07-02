namespace iEvent.Application.DTOs.Booking
{
    public class BookingQrCodeRespDto
    {
        public string BookingCode { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
    }
}
