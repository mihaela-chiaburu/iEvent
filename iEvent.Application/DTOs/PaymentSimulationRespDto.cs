using iEvent.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs
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
