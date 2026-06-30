using iEvent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
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
