using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs.Booking
{
    public class BookingFilterDto
    {
        public BookingStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Search { get; set; } 
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
