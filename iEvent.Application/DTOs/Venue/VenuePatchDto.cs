using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEvent.Application.DTOs.Venue
{
    public class VenuePatchDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public List<VenueFacilityCreateDto> Facilities { get; set; } = new();
    }
}
