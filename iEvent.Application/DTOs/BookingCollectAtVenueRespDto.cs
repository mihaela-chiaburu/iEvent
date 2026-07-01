using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class BookingCollectAtVenueRespDto : BookingRespDto
    {
        public Guid CollectedById { get; set; }
        public double CollectedAmount { get; set; }
    }
}
