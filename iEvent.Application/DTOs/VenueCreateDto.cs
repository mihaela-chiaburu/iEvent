using System;
using System.Collections.Generic;
using System.Text;

namespace iEvent.Application.DTOs
{
    public class VenueCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
