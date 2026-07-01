using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class BookingCollectAtVenueDto
    {
        public DateTime CollectedAt { get; set; }
        public decimal Amount { get; set; }
    }
}
