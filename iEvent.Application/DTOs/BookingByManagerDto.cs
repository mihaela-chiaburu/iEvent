using iEvent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
{
    public class BookingByManagerDto
    {
        public Guid EventId { get; set; }
        public Guid BookingTimeSlotId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<BookingManagerTicketDto> Tickets { get; set; } = new();
        public Guid? CustomerId { get; set; }
        public NewCustomerDto? NewCustomer { get; set; }
    }
}
