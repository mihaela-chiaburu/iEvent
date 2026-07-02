using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs.Booking
{
    public class BookingTicketAddDto
    {
        [Required]
        public Guid TicketTypeId { get; set; }

        [Range(1, 1000000, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
