using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs.Payment
{
    public class PaymentSimulationRespDto
    {
        public Guid BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public bool PaymentSucceeded { get; set; }
        public DateTime? PaidAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
