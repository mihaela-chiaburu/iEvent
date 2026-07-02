using System.ComponentModel.DataAnnotations;

namespace iEvent.Application.DTOs.Booking
{
    public class BookingTicketUpdateQuantityDto
    {
        [Range(1, 1000000, ErrorMessage = "Quantity must be at least 1.")]
        public int NewQuantity { get; set; }
    }
}
