using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs.Booking
{
    public class BookingManagerTicketDto
    {
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}
