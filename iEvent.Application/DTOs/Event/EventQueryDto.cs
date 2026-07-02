using iEvent.Domain.Enums;

namespace iEvent.Application.DTOs.Event
{
    public class EventQueryDto
    {
        public string? City { get; set; }
        public Guid? VenueId { get; set; }
        public EventCategory? Category { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } 
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
